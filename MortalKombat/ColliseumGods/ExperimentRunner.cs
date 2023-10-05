using Nsu.MortalKombat.Contracts;
using Nsu.MortalKombat.Contracts.Cards;

namespace Gods
{
    public record ExperimentResult
	{
		public bool AllowFight;

		public int Pick1;
		public int Pick2;
	}

	public class ExperimentRunner
	{
		DeckShuffler deckShuffler = new DeckShuffler();

		public ExperimentResult RunSingle(IPlayer p1, IPlayer p2)
		{
			Card[] deck = deckShuffler.GetShuffledDeck();

			// TODO: Stub; need to actually split deck into two
			Card[] deckHalf1 = new Card[18];
			Array.Copy(deck, 0, deckHalf1, 0, 18);

			Card[] deckHalf2 = new Card[18];
            Array.Copy(deck, 18, deckHalf2, 0, 18);

            // TODO: Look into how NULLs work in C#; might be able to just chain calls together
            ICardPickStrategy strat1 = p1.GetStrategy(deckHalf1);
			ICardPickStrategy strat2 = p2.GetStrategy(deckHalf2);

			int pick1 = strat1.Pick(deckHalf1);
			int pick2 = strat2.Pick(deckHalf2);

			bool allow = deckHalf1[pick2].Color == deckHalf2[pick1].Color;

			ExperimentResult res = new ExperimentResult();
			res.AllowFight = allow; // TODO: Actually check colors
			res.Pick1 = pick1;
			res.Pick2 = pick2;

			return res;
		}
	}
}