using Contracts.Cards;
using Nsu.MortalKombat.Web.Contracts;

namespace MQPlayerRoom;

// Sent by the gods when the player needs to pick the opponent's number
public class MQPickRequest: PickRequest
{
}

// Sent by the players as a response, telling what card in the opponent's deck should be picked
public class MQPickResponse
{
	public int Pick = 0;
}

// Sent by the gods, telling the player to show the N-th card
public class HTTPRevealCardRequest
{
	public Guid GameGuid { get; set; }
	public int Pick { get; set; } = 0;
}

// Sent by the gods, telling the player to show the N-th card
public class HTTPRevealCardResponse
{
	public Card Card { get; set; }
}