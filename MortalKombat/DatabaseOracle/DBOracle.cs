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
	public DbSet<DeckEntry> decks { get; set; }
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

		modelBuilder
			.Entity<DeckEntry>()
			.Property(e => e.Deck)
			.HasConversion(deckConverter);
	}

	public void StoreExperiment(ExperimentEntry entry)
	{
		Add(entry);
	}

	public void StoreDeck(Card[] deck)
	{
		Add(new DeckEntry()
		{
			Deck = deck
		});
	}
	
	public void Flush()
	{
		SaveChanges();
	}

	public ExperimentEntry? GetExperiment(int id)
	{
		return experiments.Find(id);
	}
	
	public Card[]? GetDeck(int id)
	{
		var deck = decks.Find(id);
		if (deck == null) return null;

		return deck.Deck;
	}
	/*
	 protected override void OnConfiguring(DbContextOptionsBuilder options)
	    => options.UseSqlite($"Data Source={DbPath}");
	    */
}