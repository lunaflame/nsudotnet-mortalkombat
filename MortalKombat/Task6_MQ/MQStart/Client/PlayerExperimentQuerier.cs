using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Contracts.Cards;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Nsu.MortalKombat.Web.Contracts;
using Spectre.Console;
using Util;

namespace GodClient;

public class PlayerExperimentQuerier
{
	private IBus transitBus;

	private String queue1 = "queue:ElonPlayer"; // these should be a config IMO
	private String queue2 = "queue:ZuccPlayer"; // on the other hand, he doesn't really look at the code lol 

	public PlayerExperimentQuerier(IBus bus)
	{
		transitBus = bus;
	}

	private async Task<PlayerChoice> sendToPlayer(string uri, PickRequest req)
	{
		var endpoint = await transitBus.GetSendEndpoint(new Uri(uri));
		await endpoint.Send(req);

		return null;
	}
	
    public async Task<PlayerChoice[]> SendExperiments(ExperimentDecks decks)
    {
	    PlayerChoice?[] choices = Array.Empty<PlayerChoice>();

	    sendToPlayer(queue1, new PickRequest()
    		{
    			ExperimentAmount = decks.Decks2.Count,
    			DeckBinary = DeckSerializer.SerializeDecks(decks.Decks2.ToArray())
    		});
	    
	    sendToPlayer(queue2, new PickRequest()
    		{
    			ExperimentAmount = decks.Decks2.Count,
    			DeckBinary = DeckSerializer.SerializeDecks(decks.Decks2.ToArray())
    		});
	    
    	return choices;
    }
    
    public static void ValidateResponse(PlayerChoice ch, IEnumerable<Card[]> decks)
    {
    	if (ch.CardPicks.Length != decks.Count())
    	{
    		throw new InvalidDataException($"Player responded with {ch.CardPicks.Length} picks; " +
    		                                "{decks.Decks1.Count} decks were dealt.");
    	}
    
    	var deckLength = decks.First().Length;
    	foreach (int pick in ch.CardPicks)
    	{
    		if (pick < 0 || pick >= deckLength)
    		{
    			throw new IndexOutOfRangeException($"Player tried to pick card #{pick}, but deck size is {deckLength}.");
    		}
    	}
    }
    
    public static bool ValidatePlayer(PlayerChoice? ch, List<Card[]> decks, int plyNum)
    {
    	if (ch == null)
    	{
    		AnsiConsole.MarkupLine($"[red]Player #{plyNum} failed to respond within {Config.WaitTimeout}ms.[/]");
    		return false;
    	}
    	
    	try
    	{
    		ValidateResponse(ch, decks);
    	}
    	catch (Exception e)
    	{
    		AnsiConsole.MarkupLine($"[red]Player #{plyNum} sent invalid response: {e.Message}[/]");
    		return false;
    	}
    
    	return true;
    }

    public class PlayerListener : IConsumer<MQReady>
    {
	    public Task Consume(ConsumeContext<MQReady> context)
	    {
		    Console.WriteLine($"user ready to get picked {context.Host}");
		    throw new NotImplementedException();
	    }
    }
}