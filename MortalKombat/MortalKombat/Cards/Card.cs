namespace Nsu.MortalKombat.Contracts.Cards;

/// <summary>
/// Игральная карта
/// </summary>
public record Card(CardColor Color)
{
	public override string ToString()
	{
		return Color == CardColor.Black ? "♠️" : "♦️";
	}
}