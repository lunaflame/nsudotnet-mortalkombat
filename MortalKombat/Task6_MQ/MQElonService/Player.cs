using Contracts.Interfaces;
using MassTransit;

namespace MQElonService;

public class ElonPlayer : MQPlayerRoom.PlayerRoom
{
	public ElonPlayer(IBus bus, IPlayer player) : base(bus, player)
	{
		
	}
}