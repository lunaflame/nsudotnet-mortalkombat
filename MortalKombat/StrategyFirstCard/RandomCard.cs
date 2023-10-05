using Contracts;
using Contracts.Cards;

namespace ColliseumStrategies
{
    public class RandomCard : ICardPickStrategy
	{
		private static Random rnd = new Random();

		public int Pick(Card[] cards)
		{
			return rnd.Next(0, IDeckShuffler.DeckLength);
		}
	}
}