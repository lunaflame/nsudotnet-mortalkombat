using Contracts.Cards;

namespace Contracts.Interfaces;

public interface IDeckShuffler
{
	public const int DeckLength = 36;
	public Card[] GetShuffledDeck();
}