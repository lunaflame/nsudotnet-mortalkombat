using Contracts.Cards;
using Contracts.Interfaces;
using Nsu.MortalKombat.DeckShufflers;

namespace Nsu.MortalKombat.Gods
{
    public record ExperimentResult
	{
		public bool AllowFight;

		public int Pick1;
		public int Pick2;
	}

	public class ExperimentRunner
	{
		IDeckShuffler deckShuffler = new DeckShuffler();

		public ExperimentRunner(IDeckShuffler shuffler)
		{
			this.deckShuffler = shuffler;
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

			int pick1 = strat1.Pick(deckHalf1);
			int pick2 = strat2.Pick(deckHalf2);

			bool allow = deckHalf1[pick2].Color == deckHalf2[pick1].Color;

			ExperimentResult res = new ExperimentResult
			{
				AllowFight = allow, // TODO: Actually check colors
				Pick1 = pick1,
				Pick2 = pick2
			};

			return res;
		}
	}
}