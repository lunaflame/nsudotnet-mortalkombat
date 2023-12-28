using System.Collections;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Contracts.Cards;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Nsu.MortalKombat.Web;
using Nsu.MortalKombat.Web.Contracts;
using Util;

namespace PlayerRoom.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
	private readonly IPlayer player;

	public PlayerController(IPlayer ply)
	{
		player = ply;
	}

	[HttpPost(Name = "GetOpponentDeckPick")]
	public IActionResult Get([FromBody] PickRequest req)
	{
		int halfDeckSize = IDeckShuffler.DeckLength / 2;
		int halfDeckByteSize = (int)Math.Ceiling((double)halfDeckSize / 8);
		int expectedLength = req.ExperimentAmount * halfDeckByteSize;
		
		if (req.DeckBinary.Length != expectedLength)
		{
			string err =
				$"Malformed deck binary: provided size {req.DeckBinary.Length} is not equal to {expectedLength}. "
				+ $"({req.ExperimentAmount} * {halfDeckByteSize})";
			return BadRequest(err);
		}

		byte[] deckBinSlice = new byte[halfDeckByteSize];
		Card[] deck = new Card[IDeckShuffler.DeckLength / 2];
		
		int[] choices = new int[req.ExperimentAmount];
		int i = 0;
		
		for (int cur = 0; cur < req.DeckBinary.Length; cur += halfDeckByteSize, i++)
		{
			Array.Copy(req.DeckBinary, cur, deckBinSlice, 0, halfDeckByteSize);
			DeckSerializer.DeserializeDeck(deckBinSlice, halfDeckSize, deck);

			choices[i] = player.GetStrategy(deck).Pick(deck);
		}
		
		PlayerChoice output = new PlayerChoice();
		output.CardPicks = choices;
		
		return Ok(output);
	}
}