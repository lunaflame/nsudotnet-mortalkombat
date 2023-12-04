using Contracts.Cards;
using DatabaseOracle;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace GodClient;

public static partial class Options
{
    private static DBOracle? oracle;
    private const int NumToGen = 100;
    
    private static void InitializeOracle()
    {
        var sqlconn = new SqliteConnection("Filename=:memory:");
        sqlconn.Open();

        DbContextOptions<DBOracle> opts = new DbContextOptionsBuilder<DBOracle>()
            .UseSqlite(sqlconn)
            .Options;

        oracle = new DBOracle(opts);
        oracle.Database.EnsureCreated();

        for (int i = 0; i < NumToGen; i++)
        {
            Card[] deck = shuffler.GetShuffledDeck();

            oracle.StoreDeck(deck);
        }
        
        oracle.Flush();
    }

    public static void TryStoredExperiment()
    {
	    if (oracle == null) InitializeOracle();
        
        var which = AnsiConsole.Prompt(new TextPrompt<int>("Which experiment to run?")
            .PromptStyle("blue")
            .ValidationErrorMessage("[red]A number, please[/]")
            .Validate(n =>
            {
                return n switch
                {
                    <= 0 => ValidationResult.Error($"[red]Yeah let me just run experiment #{n} real quick[/]"),
                    > NumToGen => ValidationResult.Error($"[red]There are only {NumToGen} experiments.[/]"),
                    _ => ValidationResult.Success(),
                };
            }));

        var deck = oracle!.GetDeck(which - 1);
        if (deck == null)
        {
            AnsiConsole.MarkupLine("[red]ERROR: Deck is null... HOW[/]");
            return;
        }
        
        GodClient.Options.RunDeck(deck);
    }
}