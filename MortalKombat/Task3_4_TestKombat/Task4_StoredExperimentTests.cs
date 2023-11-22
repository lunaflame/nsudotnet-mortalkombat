using Contracts.Cards;
using Contracts.Interfaces;
using DatabaseOracle;
using DatabaseOracle.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Nsu.MortalKombat;
using Nsu.MortalKombat.DeckShufflers;
using Nsu.MortalKombat.Gods;
using Nsu.MortalKombat.Players;

namespace TestKombat;

public class Task4_StoredExperimentTests
{
	private const int ExperimentCount = 1000;
	private DBOracle db;
	private readonly DeckShuffler shuffler = new();
	private SqliteConnection sqlconn;

	[OneTimeSetUp]
	public void Init()
	{
		sqlconn = new SqliteConnection("Filename=:memory:");
		sqlconn.Open();

		DbContextOptions<DBOracle> opts = new DbContextOptionsBuilder<DBOracle>()
			.UseSqlite(sqlconn)
			.Options;

		db = new DBOracle(opts);
		db.Database.EnsureCreated();
	}

	[Test]
	[Order(1)]
	public void TestExperimentStore()
	{
		{
			IPlayer p1 = new Zucc(), p2 = new Elon(); // TODO: Should player names/order be stored?

			for (int i = 0; i < ExperimentCount; i++)
			{
				Card[] deck = shuffler.GetShuffledDeck();
				(Card[] half1, Card[] half2) = DeckShuffler.SplitDeckInHalves(deck);
				ExperimentResult expOut = ExperimentRunner.UseStrategies(
					(p1.GetStrategy(half1), half1),
					(p2.GetStrategy(half2), half2));

				ExperimentEntry res = new ExperimentEntry
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

			foreach (ExperimentEntry exp in db.experiments.AsNoTracking())
			{
				loops++;
				Assert.That(exp.Deck.Length, Is.EqualTo(IDeckShuffler.DeckLength));

				(Card[] half1, Card[] half2) = DeckShuffler.SplitDeckInHalves(exp.Deck);
				ExperimentResult expOut = ExperimentRunner.UseStrategies(
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