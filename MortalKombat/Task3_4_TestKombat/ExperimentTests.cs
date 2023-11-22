using Contracts.Cards;
using Contracts.Interfaces;
using Nsu.MortalKombat.DeckShufflers;
using Nsu.MortalKombat.Gods;
using Nsu.MortalKombat.Players;
using Nsu.MortalKombat.Strategies;

namespace TestKombat;

internal class DeckShufflerShim : IDeckShuffler
{
	private readonly DeckShuffler shuffler = new();
	public int timesShuffled { get; private set; }

	public Card[] GetShuffledDeck()
	{
		timesShuffled++;
		return ((IDeckShuffler)shuffler).GetShuffledDeck();
	}
}

public class ExperimentTests
{
	private readonly Dictionary<string, Card[]> decks = new();

	private readonly Dictionary<string, (int, CardColor)[]> deckTemplates = new()
	{
		["9B18R9B"] = new[]
		{
			(9, CardColor.Black),
			(18, CardColor.Red),
			(9, CardColor.Black)
		},

		["9B9R9B9R"] = new[]
		{
			(9, CardColor.Black),
			(9, CardColor.Red),
			(9, CardColor.Black),
			(9, CardColor.Red)
		}
	};

	private ExperimentRunner runner;
	private readonly DeckShufflerShim shuffler = new();

	[SetUp]
	public void Setup()
	{
		runner = new ExperimentRunner(shuffler);

		foreach (KeyValuePair<string, (int, CardColor)[]> deckTemplate in deckTemplates)
		{
			decks[deckTemplate.Key] = new Card[IDeckShuffler.DeckLength];
			Card[] deck = decks[deckTemplate.Key];
			int cnt = 0;
			Assert.IsNotNull(deck);

			foreach ((int, CardColor) entry in deckTemplate.Value)
				for (int i = 0; i < entry.Item1; i++)
				{
					deck[cnt] = new Card(entry.Item2);
					cnt++;
				}

			Assert.That(cnt, Is.EqualTo(IDeckShuffler.DeckLength));
		}
	}

	[Test]
	public void TestShuffledOnce()
	{
		// Test that the deck gets shuffled exactly one when running a game
		IPlayer p1 = new Elon(), p2 = new Zucc();
		runner.RunSingle(p1, p2);
		Assert.That(shuffler.timesShuffled, Is.EqualTo(1));
	}

	[Test]
	public void TestExperiment()
	{
		{
			// FirstCard strategy tests
			FirstCard stratFC = new FirstCard();
			FirstCard stratFC2 = new FirstCard();

			{
				// Halves: (9 blacks, 9 reds), (9 reds, 9 blacks) => expected loss
				(Card[] half1, Card[] half2) = DeckShuffler.SplitDeckInHalves(decks["9B18R9B"]);
				ExperimentResult res = ExperimentRunner.UseStrategies((stratFC, half1), (stratFC2, half2));
				Assert.That(res.AllowFight, Is.False);
			}
			{
				// Halves: (9 blacks, 9 reds), (9 blacks, 9 reds) => expected win
				(Card[] half1, Card[] half2) = DeckShuffler.SplitDeckInHalves(decks["9B9R9B9R"]);
				ExperimentResult res = ExperimentRunner.UseStrategies((stratFC, half1), (stratFC2, half2));
				Assert.That(res.AllowFight, Is.True);
			}
		}

		// Etc...
	}
}