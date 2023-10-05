using ColliseumStrategies;
using Contracts;
using Contracts.Cards;

namespace Nsu.MortalKombat.ColliseumPlayers
{
    public class Elon : IPlayer
	{
		FirstCard strategy = new FirstCard();

		public ICardPickStrategy GetStrategy(Card[] ownCards)
		{
			return strategy;
		}
	}
}
