using System.Text.Json.Serialization;
using Util;

namespace Nsu.MortalKombat.Web.Contracts;

public class PickRequest
{
	public int ExperimentAmount { get; set; }
		
	[JsonConverter(typeof(JsonByteArrayConverter))]
	public byte[] DeckBinary { get; set; }
}