using System.Text;
using Contracts.Cards;
using Contracts.Interfaces;

namespace Util;

public static class DeckSerializer
{
	private static void _serializeDeck(Card[] deck, MemoryStream s)
	{
		using (BinaryWriter w = new BinaryWriter(s, Encoding.Default, true))
		{
			int idx = 0;
			while (idx < deck.Length)
			{
				int b = 0;
				int iterTimes = Math.Min(8, deck.Length - idx);
				for (int bitIdx = 0; bitIdx < iterTimes; bitIdx++)
				{
					int writeBit = 1 << (7 - bitIdx);
					b |= (deck[idx].Color == CardColor.Red ? (writeBit) : 0);
					// Console.Write($"{deck[idx]} ({bitIdx} = {writeBit}), ");
					idx++;
				}
				
				w.Write((byte)b);
				// Console.WriteLine($": wrote {b}");
			}
		}
	}
	public static byte[] SerializeDeck(Card[] deck)
	{
		using (MemoryStream s = new MemoryStream())
		{
			_serializeDeck(deck, s);
			return s.ToArray();
		}
		
	}

	public static byte[] SerializeDecks(IEnumerable<Card[]> decks)
	{
		// TODO: Right now, it can waste up to 7bits for each deck
		//       We can pack it more efficiently, but, you know. There's better things to do
		using (MemoryStream s = new MemoryStream())
		{
			foreach (Card[] deck in decks)
			{
				_serializeDeck(deck, s);
			}
			return s.ToArray();
		}
	}
	
	public static void DeserializeDeck(byte[] deckBin, int deckSz, Card[] dest)
	{
		using BinaryReader r = new BinaryReader(new MemoryStream(deckBin));
		
		int idx = 0;
		
		// BinaryReader.ReadBoolean works on bytes... breh
		// we pack the cards into BITS here
		// is it gonna support anything more than just binary red/black cards? hell no
		// but it's fun :):):)
		for (int i = 0; i < deckBin.Length; i++)
		{
			byte b = r.ReadByte();
			int iterTimes = Math.Min(8, deckSz - idx);
			// Console.WriteLine($"{i} -> {b}");
			for (int bit = 0; bit < iterTimes; bit++)
			{
				bool colIsRed = (b & (1 << (7 - bit))) != 0;
				dest[idx] = new Card(colIsRed ? CardColor.Red : CardColor.Black);
				// Console.WriteLine($"{idx}: {dest[idx].ToString()}");
				idx++;
			}
		}
	}
	
	public static Card[] DeserializeDeck(byte[] deckBin, int deckSize)
	{
		Card[] ret = new Card[deckSize];
		DeserializeDeck(deckBin, deckSize, ret);
		return ret;
	}
	
	public static Card[] DeserializeDeck(byte[] deckBin)
	{
		Card[] ret = new Card[IDeckShuffler.DeckLength];
		DeserializeDeck(deckBin, IDeckShuffler.DeckLength, ret);
		return ret;
	}
};