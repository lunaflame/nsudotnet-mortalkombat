using Nsu.MortalKombat.Contracts.Cards;

namespace Nsu.MortalKombat.Contracts.Interfaces
{
    public interface IPlayer
    {
        public ICardPickStrategy GetStrategy(Card[] ownCards);
    }
}
