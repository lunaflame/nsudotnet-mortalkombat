using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Contracts.Cards;
using Contracts.Interfaces;
using GodClient;
using MassTransit;
using MQPlayerRoom;
using Nsu.MortalKombat.Gods;
using Spectre.Console;
using Util;

namespace MQStart.Client;

public class PlayerExperimentQuerier : IConsumer<MQPickResponse>
{
	private readonly IBus transitBus;

	private const string Queue1 = "queue:ElonPlayer"; // these should be a config IMO
	private const string Queue2 = "queue:ZuccPlayer"; // on the other hand, he doesn't really look at the code lol 

	private static readonly string[] PlayerUrlsFor = { 
		Config.Player2Url, // player[0]'s response will be checked against this url
		Config.Player1Url,
	};
		
	private const int PlayerCount = 2;
	
	private class GameState
	{
		public readonly Guid GameGuid = NewId.NextGuid();
		
		public readonly Guid[] Players = new Guid[PlayerCount];
		public readonly int[] Picks = new int[PlayerCount];
		public readonly Card[] PickedBy = new Card[PlayerCount];

		public GameState()
		{
			NewId.NextGuid(Players, 0, PlayerCount);
			
			for (int i = 0; i < PlayerCount; i++)
			{
				Picks[i] = -1;
				PickedBy[i] = null;
			}
		}
	}
	
	// identified by the gameGuid
	private static readonly Dictionary<Guid, GameState> Games = new Dictionary<Guid, GameState>();

	private static void OutputResults(GameState game)
	{
		ExperimentResult res = new ExperimentResult()
		{
			Pick1 = game.Picks[0],
			Pick2 = game.Picks[1],
			AllowFight = game.PickedBy[0].Color == game.PickedBy[1].Color
		};
		
		AnsiConsole.WriteLine($"Game #{game.GameGuid}");
		AnsiConsole.MarkupLine($"Player #1 picked {res.Pick1} (-> {game.PickedBy[0].Color})");
		AnsiConsole.MarkupLine($"Player #2 picked {res.Pick2} (-> {game.PickedBy[1].Color})");

		AnsiConsole.MarkupLine(res.AllowFight
			? $"[green]The fight can commence![/]"
			: $"[red]The fight will not commence.[/]");
	}
	
	public PlayerExperimentQuerier(IBus bus)
	{
		transitBus = bus;
	}

	private async void SendToPlayer(GameState game, Guid plyId, string uri, MQPickRequest req)
	{
		var endpoint = await transitBus.GetSendEndpoint(new Uri(uri));
		await endpoint.Send(req, ctx =>
		{
			ctx.ConversationId = game.GameGuid;
			ctx.CorrelationId = plyId;
		});
	}
	
    public void SendExperiments(ExperimentDecks decks)
    {
	    GameState game = new GameState();
	    Games.Add(game.GameGuid, game);
	    
	    // decks.Decks* are members, not an array, so no for loop here buwomp
	    SendToPlayer(game, game.Players[0], Queue1, new MQPickRequest()
    		{
    			ExperimentAmount = decks.Decks1.Count,
    			DeckBinary = DeckSerializer.SerializeDecks(decks.Decks1.ToArray())
    		});
	    
	    SendToPlayer(game, game.Players[1], Queue2, new MQPickRequest()
    		{
    			ExperimentAmount = decks.Decks2.Count,
    			DeckBinary = DeckSerializer.SerializeDecks(decks.Decks2.ToArray())
    		});
    }

    private static readonly HttpClient HttpClient = new HttpClient();

    private static void CompleteGame(GameState game)
    {
	    // we have a complete game; output the results
	    OutputResults(game);
    }
    
    private async void CheckFetch(GameState game)
    {
	    lock (game)
	    {
		    if (game.Picks.Contains(-1)) return;
	    }

	    // If both players in the game made their pick, request each others' cards
	    Task[] tasks = new Task[PlayerCount];
	    
	    for (int i = 0; i < PlayerCount; i++)
	    {
		    int i1 = i; // haha classic
		    tasks[i] = Task.Run(async () =>
		    {
			    var resp = await HttpClient.PostAsJsonAsync($"{PlayerUrlsFor[i1]}", new HTTPRevealCardRequest()
			    {
				    GameGuid = game.GameGuid,
				    Pick = game.Picks[i1]
			    });

			    HTTPRevealCardResponse cardResponse = await resp.Content.ReadFromJsonAsync<HTTPRevealCardResponse>();
			    game.PickedBy[i1] = cardResponse.Card;
		    });
	    }

	    await Task.WhenAll(tasks);
	    CompleteGame(game);
    }
    
	public Task Consume(ConsumeContext<MQPickResponse> context)
	{
		if (!context.ConversationId.HasValue || !context.CorrelationId.HasValue)
		{
			Console.WriteLine($"null game/player PlayerChoice received; dropping.");
			return Task.CompletedTask;
		}
		
		Guid plyGuid = (Guid)context.CorrelationId;
		Guid gameGuid = (Guid)context.ConversationId;

		if (!Games.TryGetValue(gameGuid, out GameState game))
		{
			Console.WriteLine($"invalid game guid from PlayerChoice received (${gameGuid}); dropping.");
			return Task.CompletedTask;
		}

		if (context.Message.Pick < 0 || context.Message.Pick >= (IDeckShuffler.DeckLength / 2))
		{
			Console.WriteLine($"out-of-range card pick from PlayerChoice received" +
			                  $"(0 > ${context.Message.Pick} > ${IDeckShuffler.DeckLength / 2}; dropping.");
			return Task.CompletedTask;
		}
		
		for (int i = 0; i < PlayerCount; i++)
		{
			if (game.Players[i].Equals(plyGuid))
			{
				game.Picks[i] = context.Message.Pick;
			}
		}
		
		CheckFetch(game);
		return Task.CompletedTask;
	}
}