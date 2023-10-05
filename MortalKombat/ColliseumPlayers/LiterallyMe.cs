using Contracts.Cards;
using Contracts.Interfaces;
using Nsu.MortalKombat.Strategies;

namespace Nsu.MortalKombat.Players
{
    public class LiterallyMe : IPlayer
	{
		RandomCard strategy = new RandomCard();

		public ICardPickStrategy GetStrategy(Card[] ownCards) {
			return strategy;
		}
	}
}
