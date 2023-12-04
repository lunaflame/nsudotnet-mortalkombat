using System.Diagnostics;
using Contracts.Cards;
using Nsu.MortalKombat.DeckShufflers;
using Nsu.MortalKombat.Gods;
using Nsu.MortalKombat.Web.Contracts;
using Spectre.Console;

namespace GodClient;

public static partial class Options
{	
	public static void PromptNRandomDecks()
	{
		var amt = AnsiConsole.Prompt(new TextPrompt<int>("How many experiments to run?")
			.PromptStyle("blue")
			.ValidationErrorMessage("[red]A number, please[/]")
			.Validate(n =>
			{
				return n switch
				{
					< 0 => ValidationResult.Error($"[red]Yeah let me just run {n} experiments real quick[/]"),
					> 500000 => ValidationResult.Error("[red]That's insanity[/]"), // i mean... is it?
					_ => ValidationResult.Success(),
				};
			}));
		
		Stopwatch sw = Stopwatch.StartNew();
		ExperimentDecks decks = new ExperimentDecks();
		for (int i = 0; i < amt; i++)
		{
			Card[] deck = shuffler.GetShuffledDeck();
			(Card[] half1, Card[] half2) = DeckShuffler.SplitDeckInHalves(deck);
			decks.AppendDecks(half1, half2);
		}
		sw.Stop();

		PlayerChoice?[] outs;
		try
		{
			var tsk = PlayerExperimentQuerier.SendExperiments(decks);
			outs = tsk.Result;
		}
		catch (AggregateException e)
		{
			if (e.InnerException is HttpRequestException)
			{
				AnsiConsole.MarkupLine($"[red]HTTP exception: {e.InnerException.Message}[/]");
			}
			else
			{
				AnsiConsole.MarkupLine($"[red]Unknown exception! {e.Message}[/]");
				throw e; // Bubble up for the stack trace. This isn't "normal" operation AFAIK
			}
			
			return;
		}
		
		if (!PlayerExperimentQuerier.ValidatePlayer(outs[0], decks.Decks1, 1)) return;
		if (!PlayerExperimentQuerier.ValidatePlayer(outs[1], decks.Decks2, 2)) return;

		int fightsAllowed = 0;
		
		for (int i = 0; i < amt; i++)
		{
			ExperimentResult res = ExperimentRunner.GetResult(
				(decks.Decks1[i], outs[0]!.CardPicks[i]),
				(decks.Decks2[i], outs[1]!.CardPicks[i]));

			fightsAllowed += (res.AllowFight ? 1 : 0);
		}
		
		double perc = Math.Round(((double)fightsAllowed  * 100 / amt), 2);
		AnsiConsole.MarkupLine($"[green]The fight would commence {perc}% of the time ({fightsAllowed}/{amt}).[/]");
	}
}