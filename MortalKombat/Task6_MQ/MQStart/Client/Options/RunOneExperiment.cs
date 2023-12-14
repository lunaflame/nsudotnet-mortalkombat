using System;
using System.Linq;
using System.Net.Http;
using Contracts.Cards;
using Nsu.MortalKombat.DeckShufflers;
using Nsu.MortalKombat.Gods;
using Nsu.MortalKombat.Web.Contracts;
using Spectre.Console;
using Util;

namespace GodClient;

public partial class Options
{
	public void RunDeck(Card[] deck)
	{
		(Card[] half1, Card[] half2) = DeckShuffler.SplitDeckInHalves(deck);
		ExperimentDecks decks = new ExperimentDecks();
		decks.AppendDecks(half1, half2);
    
		PlayerChoice?[] outs;
		try
		{
			var tsk = querier.SendExperiments(decks);
			tsk.Wait(Config.WaitTimeout);
			outs = tsk.Result;
		}
		catch (AggregateException e)
		{
			if (e.InnerException is HttpRequestException)
			{
				AnsiConsole.MarkupLine($"[red]HTTP exception: {e.InnerException.Message}[/]");
			}
			else
			{
				AnsiConsole.MarkupLine($"[red]Unknown exception! {e.Message}[/]");
				throw e; // Bubble up for the stack trace. This isn't "normal" operation AFAIK
			}
    		
			return;
		}
		
		Console.WriteLine("We're here 1");
		if (!PlayerExperimentQuerier.ValidatePlayer(outs.ElementAtOrDefault(0), decks.Decks1, 1)) return;
		if (!PlayerExperimentQuerier.ValidatePlayer(outs.ElementAtOrDefault(1), decks.Decks2, 2)) return;
		// outs[n] checked to not be null ^
		Console.WriteLine("We're here 2");
    	
		ExperimentResult res = ExperimentRunner.GetResult(
			(half1, outs[0]!.CardPicks[0]),
			(half2, outs[1]!.CardPicks[0]));
    	
		AnsiConsole.WriteLine($"Deck #1: {Debug.DeckToString(half1)}");
		AnsiConsole.WriteLine($"Deck #2: {Debug.DeckToString(half2)}");
    	
		AnsiConsole.MarkupLine($"Player #1 picked {res.Pick1} (-> {half2[res.Pick1].ToString()})");
		AnsiConsole.MarkupLine($"Player #2 picked {res.Pick2} (-> {half1[res.Pick2].ToString()})");
		if (res.AllowFight)
		{
			AnsiConsole.MarkupLine($"[green]The fight can commence![/]");
		}
		else
		{
			AnsiConsole.MarkupLine($"[red]The fight will not commence.[/]");
		}
	}
	
    public void RunRandomDeck()
    {
    	Card[] deck = shuffler.GetShuffledDeck();
		RunDeck(deck);    	
    }
}