using Microsoft.AspNetCore.Mvc;

namespace MQPlayerRoom;

using System.Collections;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Contracts.Cards;
using Contracts.Interfaces;
using Nsu.MortalKombat.Web;
using Nsu.MortalKombat.Web.Contracts;
using Util;

[ApiController]
[Route("[controller]")]
public class MQPickCardController : ControllerBase
{
	private readonly IPlayer player;

	public MQPickCardController(IPlayer ply)
	{
		player = ply;
	}

	[HttpPost(Name = "GetOpponentDeckPick")]
	public IActionResult Get([FromBody] HTTPRevealCardRequest req)
	{
		Card[] deck = PlayerRoom.GetDealtDeck(req.GameGuid);
		
		HTTPRevealCardResponse output = new();
		output.Card = deck[req.Pick];

		PlayerRoom.ForgetGame(req.GameGuid);
		return Ok(output);
	}
}
