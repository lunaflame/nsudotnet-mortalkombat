using System.Diagnostics;
using Contracts.Cards;
using Contracts.Interfaces;

/* TODO: This assembly only adds one DeckShuffler implementation
         Should it be in `Nsu.MortalKombat.DeckShufflers` or should it just put a class into the namespace?
*/ 

namespace Nsu.MortalKombat.DeckShufflers
{
    public class DeckShuffler : IDeckShuffler
    {
        private readonly Random rnd = new Random();

        // I really expected this to be part of std, honestly
        private void FisherYatesShuffle<T>(IList<T> arr)
        {
            int n = arr.Count - 1;

            for (int i = 0; i < n; i++)
            {
                int idx = rnd.Next(i, arr.Count);
                (arr[i], arr[idx]) = (arr[idx], arr[i]);
            }
        }

        public Card[] GetShuffledDeck()
        {
            Debug.Assert(IDeckShuffler.DeckLength % 2 == 0, "Deck length isn't even; shuffled-deck generation won't be correct!");

            Card[] ret = new Card[IDeckShuffler.DeckLength];

            for (int i = 0; i < IDeckShuffler.DeckLength; i++)
            {
                // First half of the deck will be filled with black cards, the second - with red ones
                var pickedColor = i < IDeckShuffler.DeckLength / 2 ? CardColor.Black
                                                                   : CardColor.Red;
                ret[i] = new Card(pickedColor);
            }

            FisherYatesShuffle(ret);

            return ret;
        }
        
        public static (Card[], Card[]) SplitDeckInHalves(Card[] deck)
        {
            int halfSize = IDeckShuffler.DeckLength / 2;

            Card[] deckHalf1 = new Card[halfSize];
            Array.Copy(deck, 0, deckHalf1, 0, halfSize);

            Card[] deckHalf2 = new Card[halfSize];
            Array.Copy(deck, halfSize, deckHalf2, 0, halfSize);

            return (deckHalf1, deckHalf2);
        }
    }
}