using Contracts.Cards;
using Contracts.Interfaces;
using Nsu.MortalKombat.Strategies;

namespace Nsu.MortalKombat.Players;

public class LiterallyMe : IPlayer
{
	private readonly RandomCard strategy = new();

	public ICardPickStrategy GetStrategy(Card[] ownCards)
	{
		return strategy;
	}
}