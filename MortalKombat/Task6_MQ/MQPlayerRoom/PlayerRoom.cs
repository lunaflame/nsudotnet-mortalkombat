using Contracts.Cards;
using Contracts.Interfaces;
using MassTransit;
using Nsu.MortalKombat.Web.Contracts;
using Util;

namespace MQPlayerRoom;

public class PlayerRoom : IConsumer<MQPickRequest>
{
	private IBus bus;
	private IPlayer ply;

	private class GameState
	{
		public Guid GameGuid;
		public Guid MyGuid;
		public int? OpponentPicked = null;
		public Card[] Deck;
	}
	
	private static Dictionary<Guid, GameState> Games = new();

	private void replyPick(IBus bus, GameState game)
	{
		var deck = game.Deck;
		var choice = ply.GetStrategy(deck).Pick(deck);
		
		bus.Publish(new MQPickResponse()
		{
			Pick = choice
		}, newCtx =>
		{
			newCtx.ConversationId = game.GameGuid;
			newCtx.CorrelationId = game.MyGuid;
		});
	}

	public static Card[] GetDealtDeck(Guid gameGuid)
	{
		return Games[gameGuid].Deck;
	}

	public static void ForgetGame(Guid gameGuid)
	{
		Games.Remove(gameGuid);
	}
	
	public PlayerRoom(IBus bus, IPlayer player)
	{
		this.bus = bus;
		this.ply = player;
	}

	public Task Consume(ConsumeContext<MQPickRequest> ctx)
	{
		if (!ctx.ConversationId.HasValue || !ctx.CorrelationId.HasValue)
		{
			Console.WriteLine("received PickRequest without a game or player guid!? ignoring");
			return Task.CompletedTask;
		}
		
		Card[] deck = DeckSerializer.DeserializeDeck(ctx.Message.DeckBinary, IDeckShuffler.DeckLength / 2);
		GameState game = new()
		{
			GameGuid = (Guid)ctx.ConversationId,
			MyGuid = (Guid)ctx.CorrelationId,
			Deck = deck,
		};
		
		Games.Add(game.GameGuid, game);
		replyPick(bus, game);

		return Task.CompletedTask;
	}
}