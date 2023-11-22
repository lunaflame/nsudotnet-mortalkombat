using Contracts.Cards;
using Contracts.Interfaces;
using Nsu.MortalKombat.Strategies;

namespace TestKombat;

public class StrategyTests
{
	private const int halfDeckSize = 18;

	private readonly Dictionary<string, Card[]> decks = new()
	{
		["Black18"] = new Card[] { },
		["Red18"] = new Card[] { },
		["Black9Red9"] = new Card[] { },
		["Black1Red17"] = new Card[] { }
	};

	private Dictionary<string, ICardPickStrategy> strats;

	// Generate half-decks that we'll use to test our strategies
	private void FillDecks()
	{
		for (int i = 0; i < halfDeckSize; i++)
		{
			decks["Black18"].Append(new Card(CardColor.Black));
			decks["Red18"].Append(new Card(CardColor.Red));
		}

		for (int i = 0; i < halfDeckSize; i++)
		{
			CardColor halfCol = i switch
			{
				var n when n < 9 => CardColor.Black,
				var n when n >= 9 => CardColor.Red,
				_ => throw new IndexOutOfRangeException("generating non-18-sized halfdecks???")
			};

			decks["Black9Red9"].Append(new Card(halfCol));
			decks["Black1Red17"].Append(new Card(i == 0 ? CardColor.Black : CardColor.Red));
		}
	}

	[SetUp]
	public void Setup()
	{
		strats = new Dictionary<string, ICardPickStrategy>
		{
			["FirstCard"] = new FirstCard(),
			["RandomCard"] = new RandomCard() // i mean we can't really test *random* much, can we
		};

		FillDecks();
	}

	[Test]
	public void TestStrategies()
	{
		for (int i = 0; i < 10; i++)
			// Check that FirstCard picks the first card no matter the deck
			foreach (KeyValuePair<string, Card[]> kv in decks)
				Assert.That(strats["FirstCard"].Pick(kv.Value), Is.EqualTo(0));

		// Check that RandomCard can pick different cards for the same deck
		foreach (KeyValuePair<string, Card[]> kv in decks)
		{
			int firstPick = strats["RandomCard"].Pick(kv.Value);
			bool pickedDiff = false;

			for (int i = 0; i < 1000; i++)
				// If we pick a card other than our initial pick, we pass the test
				if (strats["RandomCard"].Pick(kv.Value) != firstPick)
				{
					pickedDiff = true;
					break;
				}

			Assert.True(pickedDiff);
		}
	}
}