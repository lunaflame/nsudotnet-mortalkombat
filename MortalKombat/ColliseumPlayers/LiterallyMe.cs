using ColliseumStrategies;
using Contracts;
using Contracts.Cards;

namespace Nsu.MortalKombat.ColliseumPlayers
{
    public class LiterallyMe : IPlayer
	{
		RandomCard strategy = new RandomCard();

		public ICardPickStrategy GetStrategy(Card[] ownCards) {
			return strategy;
		}
	}
}
