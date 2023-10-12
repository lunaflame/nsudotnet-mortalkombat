using Contracts.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Nsu.MortalKombat.DatabaseOracle;
using Nsu.MortalKombat.DatabaseOracle.Models;
using Nsu.MortalKombat.DeckShufflers;
using Nsu.MortalKombat.Gods;
using Nsu.MortalKombat.Players;

namespace TestKombat;

public class StoredExperimentTests
{
    private DBOracle db;
    private DeckShuffler shuffler = new DeckShuffler();
    private SqliteConnection sqlconn;
    
    [OneTimeSetUp]
    public void Init()
    {
        sqlconn = new SqliteConnection("Filename=:memory:");
        sqlconn.Open();

        var opts = new DbContextOptionsBuilder<DBOracle>()
            .UseSqlite(sqlconn)
            .Options;

        db = new DBOracle(opts);
        db.Database.EnsureCreated();
    }
    
    private const int ExperimentCount = 100;
    
    [Test, Order(1)]
    public void TestExperimentStore()
    {
        {
            IPlayer p1 = new Zucc(), p2 = new Elon(); // TODO: Should player names/order be stored?

            for (int i = 0; i < ExperimentCount; i++)
            {
                var deck = shuffler.GetShuffledDeck();
                var (half1, half2) = DeckShuffler.SplitDeckInHalves(deck);
                var expOut = ExperimentRunner.UseStrategies(
                    (p1.GetStrategy(half1), half1),
                    (p2.GetStrategy(half2), half2));

                var res = new ExperimentEntry()
                {
                    Deck = deck,
                    Pick1 = expOut.Pick1,
                    Pick2 = expOut.Pick2,
                    AllowFight = expOut.AllowFight
                };

                db.StoreExperiment(res);
            }

            db.Flush();
            Assert.That(db.experiments.Count(), Is.EqualTo(ExperimentCount));
        }
        /*
        }
    
        [Test, Order(2)]
        public void TestRead()
        {
        */
        {
            IPlayer p1 = new Zucc(), p2 = new Elon(); // TODO: Should player names/order be stored?

            int loops = 0;
            Assert.That(db.experiments.Count(), Is.EqualTo(ExperimentCount));

            foreach (var exp in db.experiments.AsNoTracking())
            {
                loops++;
                Assert.That(exp.Deck.Length, Is.EqualTo(IDeckShuffler.DeckLength));

                var (half1, half2) = DeckShuffler.SplitDeckInHalves(exp.Deck);
                var expOut = ExperimentRunner.UseStrategies(
                    (p1.GetStrategy(half1), half1),
                    (p2.GetStrategy(half2), half2));

                Assert.That(exp.Pick1, Is.EqualTo(expOut.Pick1));
                Assert.That(exp.Pick2, Is.EqualTo(expOut.Pick2));
                Assert.That(exp.AllowFight, Is.EqualTo(expOut.AllowFight));
            }

            Assert.That(loops, Is.EqualTo(ExperimentCount));
        }
    }
}