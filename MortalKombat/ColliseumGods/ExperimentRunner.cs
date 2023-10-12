using Contracts.Cards;
using Contracts.Interfaces;
using Nsu.MortalKombat.DeckShufflers;

namespace Nsu.MortalKombat.Gods;

public record ExperimentResult
{
	public bool AllowFight;

	public int Pick1;
	public int Pick2;
}

public class ExperimentRunner
{
	private readonly IDeckShuffler deckShuffler = new DeckShuffler();

	public ExperimentRunner(IDeckShuffler shuffler)
	{
		deckShuffler = shuffler;
	}

	public static ExperimentResult UseStrategies((ICardPickStrategy, Card[]) p1, (ICardPickStrategy, Card[]) p2)
	{
		ICardPickStrategy strat1 = p1.Item1, strat2 = p2.Item1;
		Card[] deck1 = p1.Item2, deck2 = p2.Item2;

		int pick1 = strat1.Pick(deck1);
		int pick2 = strat2.Pick(deck2);

		bool allow = deck1[pick2].Color == deck2[pick1].Color;

		ExperimentResult res = new()
		{
			AllowFight = allow,
			Pick1 = pick1,
			Pick2 = pick2
		};

		return res;
	}

	public ExperimentResult RunSingle(IPlayer p1, IPlayer p2)
	{
		Card[] deck = deckShuffler.GetShuffledDeck();

		Card[] deckHalf1 = new Card[18];
		Array.Copy(deck, 0, deckHalf1, 0, 18);

		Card[] deckHalf2 = new Card[18];
		Array.Copy(deck, 18, deckHalf2, 0, 18);

		// TODO: Look into how NULLs work in C#; might be able to just chain calls together
		ICardPickStrategy strat1 = p1.GetStrategy(deckHalf1);
		ICardPickStrategy strat2 = p2.GetStrategy(deckHalf2);

		ExperimentResult res = UseStrategies((strat1, deckHalf1), (strat2, deckHalf2));

		return res;
	}
}