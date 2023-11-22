using System.Text;
using Contracts.Cards;

namespace Util;

public static class Debug
{
	public static string DeckToString(Card[] deck)
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < deck.Length; i++)
		{
			Card card = deck[i];

			if (i != 0)
				sb.Append(", ");
			sb.Append(card.Color == CardColor.Black ? "B" : "R"); // the stringbuilder commits die with emojis WTF
		}

		return sb.ToString();
	}
	
	public static void PrintDeck(Card[] deck)
	{
		Console.WriteLine($"Deck[{deck.Length}]: {DeckToString(deck)}");
	}

	public static void PrintBinary(byte[] bytes)
	{
		Console.Write($"byte[{bytes.Length}]: ");
		for (int i = 0; i < bytes.Length; i++)
		{
			if (i != 0) Console.Write("|");
			for (int bit = 0; bit < 8; bit++)
			{
				Console.Write((bytes[i] >> (7 - bit)) & 1);
			}
		}
		Console.Write("\n");
	}
}