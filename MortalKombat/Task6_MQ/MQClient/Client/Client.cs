using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GodClient;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace MQStart.Client;

public class Client : BackgroundService
{
	private Dictionary<Action, string> choiceToName;

    public Client(IBus bus)
    {
        Options opts = new Options(new PlayerExperimentQuerier(bus));
        
        
        choiceToName = new Dictionary<Action, string>
        {
	        [opts.RunRandomDeck] = "Run an experiment with a random deck",
        };
    }
    
	private bool shouldExit = false;

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