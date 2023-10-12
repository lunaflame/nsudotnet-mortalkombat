using Contracts.Cards;

namespace Contracts.Interfaces;

public interface IPlayer
{
	public ICardPickStrategy GetStrategy(Card[] ownCards);
}