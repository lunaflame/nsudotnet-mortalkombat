using Contracts.Cards;
using Contracts.Interfaces;

namespace Nsu.MortalKombat.Strategies;

public class FirstCard : ICardPickStrategy
{
	public int Pick(Card[] cards)
	{
		return 0;
	}
}