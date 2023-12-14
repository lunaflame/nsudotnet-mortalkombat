using System;
using System.Collections.Generic;
using Contracts.Cards;

namespace GodClient;

public static class Config
{
	public static string Player1Url = "http://localhost:5000";
	public static string Player2Url = "http://localhost:5001";
	public const int WaitTimeout = 2500;
	public const string GodName = "Mars"; // Roman god of war, because funny
}

public class ExperimentDecks
{
	public List<Card[]> Decks1 { get; private set; } = new List<Card[]>();
	public List<Card[]> Decks2 { get; private set; } = new List<Card[]>();
	private int deckLength = -1;

	public void AppendDecks(Card[] deck1, Card[] deck2)
	{
		if (deck1.Length != deck2.Length)
		{
			throw new Exception("Players' deck lengths aren't equal.");
		}
		
		if (deckLength >= 0 && deckLength != deck1.Length)
		{
			throw new Exception("Tried to add a deck with a size different from ones added before.");
		}
		
		Decks1.Add(deck1);
		Decks2.Add(deck2);
		deckLength = deck1.Length;
	}
}