using Contracts.Cards;
using DatabaseOracle.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nsu.MortalKombat;
using Util;

namespace DatabaseOracle;

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
			deck => DeckSerializer.SerializeDeck(deck),
			deckBin => DeckSerializer.DeserializeDeck(deckBin)
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
	/*
	 protected override void OnConfiguring(DbContextOptionsBuilder options)
	    => options.UseSqlite($"Data Source={DbPath}");
	    */
}