using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace GodClient;

public class Client : BackgroundService // is it really a *background* service, though
{
	readonly IBus transitBus;
	private PlayerExperimentQuerier querier;
	private Options opts;
	private Dictionary<Action, string> choiceToName;

    public Client(IBus bus)
    {
        transitBus = bus;
        querier = new PlayerExperimentQuerier(bus);
        opts = new Options(querier);
        
        choiceToName = new Dictionary<Action, string>
        {
	        [opts.RunRandomDeck] = "Run an experiment with a random deck",
	        // [opts.PromptNRandomDecks] = "Run N experiments with random decks",
	        // [opts.TryStoredExperiment] = "Run a random stored experiment",
        };
    }
    
	private bool shouldExit = false;
	private void Exit()
	{
		shouldExit = true;
	}

	string ConvertChoiceToText(Action choice)
	{
		string ret;
		if (!choiceToName.TryGetValue(choice, out ret))
		{
			ret = $"[unregistered choice: {choice.Method.Name}]".EscapeMarkup();
		}
		
		return ret;
	}
	
	public void QueryChoice(Action[] choices)
	{
		Action choice = AnsiConsole.Prompt(
			new SelectionPrompt<Action>()
				.Title($"[green][[{Config.GodName}]][/] What are we doing?")
				.PageSize(10)
				.MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
				.UseConverter(ConvertChoiceToText)
				.AddChoices(choices)
		);

		choice();
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		return Task.Run(() =>
		{
			Action[] choices = choiceToName.Keys.ToArray();
				
			// The "Exit" choice sets `shouldExit` to true; that breaks us out of the while loop
			while (!shouldExit)
			{
				QueryChoice(choices);
			}
		}, stoppingToken);
	}
}