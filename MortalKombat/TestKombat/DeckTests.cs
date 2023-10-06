using Contracts.Cards;
using Contracts.Interfaces;
using Nsu.MortalKombat.DeckShufflers;

namespace TestKombat;

public class DeckTests
{
    private IDeckShuffler shuffler;
    
    [SetUp]
    public void Setup()
    {
        shuffler = new DeckShuffler();
    }

    [Test]
    public void TestShuffledCards()
    {
        Card[] deck = shuffler.GetShuffledDeck();

        int blackCards = deck.Count(card => card.Color == CardColor.Black),
            redCards   = deck.Count(card => card.Color == CardColor.Red);

        Assert.That(blackCards, Is.EqualTo(18));
        Assert.That(redCards, Is.EqualTo(18));
    }
}