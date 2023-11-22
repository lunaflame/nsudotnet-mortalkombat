using System.Diagnostics;
using System.Net.Http.Json;
using Contracts.Cards;
using GodClient;
using Nsu.MortalKombat;
using Nsu.MortalKombat.DeckShufflers;
using Nsu.MortalKombat.Gods;
using Nsu.MortalKombat.Web.Contracts;
using Spectre.Console;
using Util;
using Debug = Util.Debug;

DeckShuffler shuffler = new DeckShuffler();
HttpClient httpClient = new HttpClient();

string p1Url = "http://localhost:5000";
string p2Url = "http://localhost:5001";
const string godName = "Mars"; // Roman god of war, because funny

const int waitTimeout = 2500;

async Task<PlayerChoice?[]> SendExperiments(ExperimentDecks decks)
{
	Console.WriteLine("blacks 1");
	HttpResponseMessage[] resps = await Task.WhenAll<HttpResponseMessage>(new[] {
		httpClient.PostAsJsonAsync($"{p1Url}/Player", new PickRequest()
		{
			ExperimentAmount = decks.Decks1.Count,
			DeckBinary = DeckSerializer.SerializeDecks(decks.Decks1.ToArray())
		}),
		
		httpClient.PostAsJsonAsync($"{p2Url}/Player", new PickRequest()
		{
			ExperimentAmount = decks.Decks2.Count,
			DeckBinary = DeckSerializer.SerializeDecks(decks.Decks2.ToArray())
		}),
	});
	Console.WriteLine("blacks 2");


	PlayerChoice?[] choices = await Task.WhenAll<PlayerChoice?>(new[]
	{
		resps[0].Content.ReadFromJsonAsync<PlayerChoice>(),
		resps[1].Content.ReadFromJsonAsync<PlayerChoice>()
	});
	
	return choices;
}

void ValidateResponse(PlayerChoice ch, IEnumerable<Card[]> decks)
{
	if (ch.CardPicks.Length != decks.Count())
	{
		throw new InvalidDataException($"Player responded with {ch.CardPicks.Length} picks; " +
		                                "{decks.Decks1.Count} decks were dealt.");
	}

	var deckLength = decks.First().Length;
	foreach (int pick in ch.CardPicks)
	{
		if (pick < 0 || pick >= deckLength)
		{
			throw new IndexOutOfRangeException($"Player tried to pick card #{pick}, but deck size is {deckLength}.");
		}
	}
}

bool ValidatePlayer(PlayerChoice? ch, List<Card[]> decks, int plyNum)
{
	if (ch == null)
	{
		AnsiConsole.MarkupLine($"[red]Player #{plyNum} failed to respond within {waitTimeout}ms.[/]");
		return false;
	}
	
	try
	{
		ValidateResponse(ch, decks);
	}
	catch (Exception e)
	{
		AnsiConsole.MarkupLine($"[red]Player #{plyNum} sent invalid response: {e.Message}[/]");
		return false;
	}

	return true;
}

void RunRandomDeck()
{
	Card[] deck = shuffler.GetShuffledDeck();
	(Card[] half1, Card[] half2) = DeckShuffler.SplitDeckInHalves(deck);
	ExperimentDecks decks = new ExperimentDecks();
	decks.AppendDecks(half1, half2);

	PlayerChoice?[] outs;
	try
	{
		var tsk = SendExperiments(decks);
		tsk.Wait(waitTimeout);
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
	
	if (!ValidatePlayer(outs[0], decks.Decks1, 1)) return;
	if (!ValidatePlayer(outs[1], decks.Decks2, 2)) return;
	// outs[n] checked to not be null ^
	
	ExperimentResult res = ExperimentRunner.GetResult(
		(half1, outs[0]!.CardPicks[0]),
		(half2, outs[1]!.CardPicks[0]));
	
	AnsiConsole.WriteLine($"Deck #1: {Debug.DeckToString(half1)}");
	AnsiConsole.WriteLine($"Deck #2: {Debug.DeckToString(half2)}");
	
	AnsiConsole.MarkupLine($"Player #1 picked {res.Pick1} (-> {half2[res.Pick1].ToString()})");
	AnsiConsole.MarkupLine($"Player #2 picked {res.Pick2} (-> {half1[res.Pick2].ToString()})");
	if (res.AllowFight)
	{
		AnsiConsole.MarkupLine($"[green]The fight can commence![/]");
	}
	else
	{
		AnsiConsole.MarkupLine($"[red]The fight will not commence.[/]");
	}
}

void PromptNRandomDecks()
{
	var amt = 250000; /* AnsiConsole.Prompt(new TextPrompt<int>("How many experiments to run?")
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
		}));*/
	
	Console.WriteLine("blacks 0");
	Stopwatch sw = Stopwatch.StartNew();
	ExperimentDecks decks = new ExperimentDecks();
	for (int i = 0; i < amt; i++)
	{
		Card[] deck = shuffler.GetShuffledDeck();
		(Card[] half1, Card[] half2) = DeckShuffler.SplitDeckInHalves(deck);
		decks.AppendDecks(half1, half2);
	}
	sw.Stop();
	Console.WriteLine("blacks 0.5 " + sw.ElapsedMilliseconds);
	
	PlayerChoice?[] outs;
	try
	{
		var tsk = SendExperiments(decks);
		tsk.Wait(waitTimeout);
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
	
	if (!ValidatePlayer(outs[0], decks.Decks1, 1)) return;
	if (!ValidatePlayer(outs[1], decks.Decks2, 2)) return;

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

bool shouldIter = true;
void Exit()
{
	AnsiConsole.Markup("[red]Exit requested.[/]");
	// Environment.Exit(0);
	shouldIter = false;
}

Dictionary<Action, string> choiceToName = new Dictionary<Action, string>
{
	[RunRandomDeck] = "Run an experiment with a random deck",
	[PromptNRandomDecks] = "Run N experiments with random decks",
	[Exit] = "Exit",
};

string ConvertChoiceToText(Action choice)
{
	string ret = "";
	if (!choiceToName.TryGetValue(choice, out ret))
	{
		ret = $"[unregistered choice: {choice.Method.Name}]".EscapeMarkup();
	}
	
	return ret;
}

while (shouldIter)
{
	var choice = PromptNRandomDecks;
	
	/*var choice = AnsiConsole.Prompt(
		new SelectionPrompt<Action>()
			.Title($"[green][[{godName}]][/] What are we doing?")
			.PageSize(10)
			.MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
			.UseConverter(ConvertChoiceToText)
			.AddChoices(new[] {
				RunRandomDeck,
				PromptNRandomDecks,
				Exit
			}));*/

	choice();
}
