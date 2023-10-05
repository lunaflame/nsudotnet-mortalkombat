using ColliseumStrategies;
using Contracts;
using Contracts.Cards;

namespace Nsu.MortalKombat.ColliseumPlayers
{
    public class Zucc : IPlayer
	{
		FirstCard strategy = new FirstCard();

		public ICardPickStrategy GetStrategy(Card[] ownCards)
		{
			return strategy;
		}
	}
}
