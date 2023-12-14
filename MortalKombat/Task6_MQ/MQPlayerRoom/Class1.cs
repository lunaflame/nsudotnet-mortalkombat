using Contracts.Cards;
using Contracts.Interfaces;
using MassTransit;
using Nsu.MortalKombat.Web.Contracts;
using Util;

namespace MQPlayerRoom;

public class PlayerRoom : IConsumer<PickRequest>, IConsumer<PlayerChoice>
{
	private IBus bus;
	private IPlayer ply;
	private static Guid guid = Guid.NewGuid();
	private static String mqGodName = "PlayerListener"; // TODO (won't actually be done LMAO): config
	private static object lockDummy = new object();
	private static Card[] deckDealt;
	private static int pickNumber = -1;

	private static void checkPick(IBus bus)
	{
		lock (lockDummy)
		{
			if (deckDealt == null || pickNumber == -1) return;

			Card picked = deckDealt[pickNumber];
			deckDealt = null;
			pickNumber = -1;
		}
		
		Task.Run(async () =>
		{
			Console.WriteLine("both given; sending ready");
			var ep = await bus.GetSendEndpoint(new Uri(mqGodName));
			ep.Send(new MQReady());
		});
	}
	
	public PlayerRoom(IBus bus, IPlayer player)
	{
		this.bus = bus;
		this.ply = player;
	}

	public Task Consume(ConsumeContext<PickRequest> context)
	{
		Console.WriteLine($"mmm yummy data {context.Message.ExperimentAmount} => {context.Message.DeckBinary.Length}bytes");
		Card[] deck = DeckSerializer.DeserializeDeck(context.Message.DeckBinary, IDeckShuffler.DeckLength / 2);
		lock (lockDummy)
		{
			deckDealt = deck;
		}
		
		checkPick(bus);

		var choice = ply.GetStrategy(deck).Pick(deck);

		return bus.Publish(new PlayerChoice()
		{
			CardPicks = new int[1] { choice }
		}, (h) =>
		{
			h.Headers.Set("guid", guid.ToString());
		});
	}

	public Task Consume(ConsumeContext<PlayerChoice> context)
	{
		if (context.Headers.Get<string>("guid", "")!.Equals(guid.ToString()))
		{
			return Task.CompletedTask;
		}
		
		lock (lockDummy)
		{
			pickNumber = context.Message.CardPicks[0];
		}
		
		checkPick(bus);
		
		return Task.CompletedTask;
	}
}