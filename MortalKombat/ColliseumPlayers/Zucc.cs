using Contracts.Cards;
using Contracts.Interfaces;
using Nsu.MortalKombat.Strategies;

namespace Nsu.MortalKombat.Players;

public class Zucc : IPlayer
{
	private readonly FirstCard strategy = new();

	public ICardPickStrategy GetStrategy(Card[] ownCards)
	{
		return strategy;
	}
}