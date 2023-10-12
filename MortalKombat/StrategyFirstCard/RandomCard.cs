using Contracts.Cards;
using Contracts.Interfaces;

namespace Nsu.MortalKombat.Strategies;

public class RandomCard : ICardPickStrategy
{
	private static readonly Random rnd = new();

	public int Pick(Card[] cards)
	{
		return rnd.Next(0, IDeckShuffler.DeckLength);
	}
}