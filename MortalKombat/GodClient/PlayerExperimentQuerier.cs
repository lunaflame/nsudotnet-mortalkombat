using System.Net.Http.Json;
using Contracts.Cards;
using Nsu.MortalKombat.Web.Contracts;
using Spectre.Console;
using Util;

namespace GodClient;

public static class PlayerExperimentQuerier
{
	public static HttpClient httpClient = new HttpClient();
	
    public static async Task<PlayerChoice?[]> SendExperiments(ExperimentDecks decks)
    {
    	HttpResponseMessage[] resps = await Task.WhenAll<HttpResponseMessage>(new[] {
    		httpClient.PostAsJsonAsync($"{Config.Player1Url}/Player", new PickRequest()
    		{
    			ExperimentAmount = decks.Decks1.Count,
    			DeckBinary = DeckSerializer.SerializeDecks(decks.Decks1.ToArray())
    		}),
    		
    		httpClient.PostAsJsonAsync($"{Config.Player2Url}/Player", new PickRequest()
    		{
    			ExperimentAmount = decks.Decks2.Count,
    			DeckBinary = DeckSerializer.SerializeDecks(decks.Decks2.ToArray())
    		}),
    	});
    
    	PlayerChoice?[] choices = await Task.WhenAll<PlayerChoice?>(new[]
    	{
    		resps[0].Content.ReadFromJsonAsync<PlayerChoice>(),
    		resps[1].Content.ReadFromJsonAsync<PlayerChoice>()
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
}