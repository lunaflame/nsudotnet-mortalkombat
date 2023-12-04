using GodClient;
using Spectre.Console;

bool shouldIter = true;
void Exit()
{
	AnsiConsole.Markup("[red]Exit requested.[/]");
	// Environment.Exit(0);
	shouldIter = false;
}

Dictionary<Action, string> choiceToName = new Dictionary<Action, string>
{
	[Options.RunRandomDeck] = "Run an experiment with a random deck",
	[Options.PromptNRandomDecks] = "Run N experiments with random decks",
	[Options.TryStoredExperiment] = "Run a random stored experiment",
	[Exit] = "Exit",
};

string ConvertChoiceToText(Action choice)
{
	string ret;
	if (!choiceToName.TryGetValue(choice, out ret))
	{
		ret = $"[unregistered choice: {choice.Method.Name}]".EscapeMarkup();
	}
	
	return ret;
}

while (shouldIter)
{
	Action[] choices = choiceToName.Keys.ToArray();
	
	var choice = AnsiConsole.Prompt(
		new SelectionPrompt<Action>()
			.Title($"[green][[{Config.GodName}]][/] What are we doing?")
			.PageSize(10)
			.MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
			.UseConverter(ConvertChoiceToText)
			.AddChoices(choices)
		);

	choice();
}
