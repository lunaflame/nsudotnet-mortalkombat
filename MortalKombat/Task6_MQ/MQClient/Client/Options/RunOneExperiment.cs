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
    
		querier.SendExperiments(decks);
		AnsiConsole.MarkupLine($"[yellow]Experiment submitted, awaiting reply...[/]");
	}
	
    public void RunRandomDeck()
    {
    	Card[] deck = shuffler.GetShuffledDeck();
		RunDeck(deck);    	
    }
}