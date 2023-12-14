using Contracts.Interfaces;
using MassTransit;

namespace MQZuccService;

public class ZuccPlayer : MQPlayerRoom.PlayerRoom
{
	public ZuccPlayer(IBus bus, IPlayer player) : base(bus, player)
	{
		
	}
}