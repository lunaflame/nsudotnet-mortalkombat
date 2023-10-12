using Contracts.Cards;
using Contracts.Interfaces;
using Nsu.MortalKombat.DeckShufflers;
using Nsu.MortalKombat.Strategies;
using Nsu.MortalKombat.Gods;
using System.Text;
using Nsu.MortalKombat.Players;

namespace TestKombat;

class DeckShufflerShim : IDeckShuffler
{
    public int timesShuffled { get; private set; } = 0;
    private DeckShuffler shuffler = new DeckShuffler();

    public Card[] GetShuffledDeck()
    {
        timesShuffled++;
        return ((IDeckShuffler)shuffler).GetShuffledDeck();
    }
}

public class ExperimentTests
{

    private Dictionary<String, Card[]> decks = new Dictionary<String, Card[]> ();
    private DeckShufflerShim shuffler = new DeckShufflerShim();
    private ExperimentRunner runner;

    private Dictionary<String, (int, CardColor)[]> deckTemplates = new Dictionary<String, (int, CardColor)[]>
    {
        ["9B18R9B"] = new[] {
            ( 9, CardColor.Black ),
            ( 18, CardColor.Red ),
            ( 9, CardColor.Black ),
        },

        ["9B9R9B9R"] = new[] {
            ( 9, CardColor.Black ),
            ( 9, CardColor.Red ),
            ( 9, CardColor.Black ),
            ( 9, CardColor.Red ),
        },
    };

    [SetUp]
    public void Setup()
    {
        runner = new ExperimentRunner(shuffler);

        foreach (var deckTemplate in deckTemplates)
        {
            decks[deckTemplate.Key] = new Card[IDeckShuffler.DeckLength];
            var deck = decks[deckTemplate.Key];
            int cnt = 0;
            Assert.IsNotNull(deck);

            foreach (var entry in deckTemplate.Value)
            {
                for (int i = 0; i < entry.Item1; i++)
                {
                    deck[cnt] = new Card(entry.Item2);
                    cnt++;
                }
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
        { // FirstCard strategy tests
            var stratFC = new FirstCard();
            var stratFC2 = new FirstCard();

            { // Halves: (9 blacks, 9 reds), (9 reds, 9 blacks) => expected loss
                var (half1, half2) = DeckShuffler.SplitDeckInHalves(decks["9B18R9B"]);
                var res = ExperimentRunner.UseStrategies((stratFC, half1), (stratFC2, half2));
                Assert.That(res.AllowFight, Is.False);
            }
            { // Halves: (9 blacks, 9 reds), (9 blacks, 9 reds) => expected win
                var (half1, half2) = DeckShuffler.SplitDeckInHalves(decks["9B9R9B9R"]);
                var res = ExperimentRunner.UseStrategies((stratFC, half1), (stratFC2, half2));
                Assert.That(res.AllowFight, Is.True);
            }
        }

        // Etc...
    }
}