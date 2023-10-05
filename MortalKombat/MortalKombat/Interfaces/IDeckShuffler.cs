using Nsu.MortalKombat.Contracts.Cards;

namespace Nsu.MortalKombat.Contracts.Interfaces;

public interface IDeckShuffler
{
    public const int DeckLength = 36;
    public Card[] GetShuffledDeck();
}
