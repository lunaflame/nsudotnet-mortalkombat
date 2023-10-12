using Contracts.Cards;
using Contracts.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nsu.MortalKombat.DatabaseOracle.Models;

namespace Nsu.MortalKombat.DatabaseOracle;

public class DBOracle : DbContext
{
	public string DbPath { get; }
	public DbSet<ExperimentEntry> experiments { get; set; }
	private readonly ValueConverter<Card[], byte[]> deckConverter;


	public DBOracle(DbContextOptions<DBOracle> options)
		: base(options)
	{
		using MemoryStream s = new MemoryStream();
		using BinaryReader r = new BinaryReader(s);
		deckConverter = new ValueConverter<Card[], byte[]>(
			deck => SerializeDeck(deck),
			deckBin => DeserializeDeck(deckBin)
		);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder
			.Entity<ExperimentEntry>()
			.Property(e => e.Deck)
			.HasConversion(deckConverter);
	}

	public void StoreExperiment(ExperimentEntry entry)
	{
		Add(entry);
	}

	public void Flush()
	{
		SaveChanges();
	}

	public ExperimentEntry GetExperiment(int id)
	{
		return experiments.Find(id);
	}

	private byte[] SerializeDeck(Card[] deck)
	{
		using (MemoryStream s = new MemoryStream())
		using (BinaryWriter w = new BinaryWriter(s))
		{
			for (int i = 0; i < IDeckShuffler.DeckLength; i++) w.Write(deck[i].Color == CardColor.Red);

			return s.ToArray();
		}
	}

	private Card[] DeserializeDeck(byte[] deckBin)
	{
		Card[] ret = new Card[IDeckShuffler.DeckLength];

		using BinaryReader r = new BinaryReader(new MemoryStream(deckBin));
		for (int i = 0; i < IDeckShuffler.DeckLength; i++)
		{
			bool colIsRed = r.ReadBoolean();
			ret[i] = new Card(colIsRed ? CardColor.Red : CardColor.Black);
		}

		return ret;
	}
	/*
	 protected override void OnConfiguring(DbContextOptionsBuilder options)
	    => options.UseSqlite($"Data Source={DbPath}");
	    */
}