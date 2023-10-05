using Contracts.Cards;
using System.Diagnostics;

namespace Contracts
{

    public class DeckShuffler : IDeckShuffler
    {
        private Random rnd = new Random();

        // I really expected this to be part of std, honestly
        private void FisherYatesShuffle<T>(T[] arr)
        {
            int n = arr.Length - 1;

            for (int i = 0; i < n; i++)
            {
                int idx = rnd.Next(i, arr.Length);
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
                CardColor pickedColor = i < IDeckShuffler.DeckLength / 2 ? CardColor.Black
                                                                         : CardColor.Red;
                ret[i] = new Card(pickedColor);
            }

            FisherYatesShuffle(ret);

            return ret;
        }

    }
}