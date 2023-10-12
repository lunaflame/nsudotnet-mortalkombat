using System.ComponentModel.DataAnnotations.Schema;
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
    private ValueConverter<Card[], byte[]> deckConverter;

    
    public DBOracle(DbContextOptions<DBOracle> options)
        : base(options)
    {
        using var s = new MemoryStream();
        using var r = new BinaryReader(s);
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
        this.Add(entry);
    }
    
    public void Flush()
    {
        this.SaveChanges();
    }

    public ExperimentEntry GetExperiment(int id)
    {
        return this.experiments.Find(id);
    }
    
    private byte[] SerializeDeck(Card[] deck)
    {
        using(var s = new MemoryStream())
        using(var w = new BinaryWriter(s))
        {
            for (int i = 0; i < IDeckShuffler.DeckLength; i++)
            {
                w.Write(deck[i].Color == CardColor.Red);
            }

            return s.ToArray();
        }
    }
    
    private Card[] DeserializeDeck(byte[] deckBin)
    {
        var ret = new Card[IDeckShuffler.DeckLength];
        
        using var r = new BinaryReader(new MemoryStream(deckBin));
        for (int i = 0; i < IDeckShuffler.DeckLength; i++)
        { 
            var colIsRed = r.ReadBoolean();
            ret[i] = new Card(colIsRed ? CardColor.Red : CardColor.Black);
        }

        return ret;
    }
    /*
     protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
        */
}