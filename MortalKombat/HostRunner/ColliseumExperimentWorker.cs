using Contracts.Interfaces;
using Nsu.MortalKombat.Gods;

namespace HostRunner;

public class ColliseumExperimentWorker : IHostedService
{
    private ExperimentRunner runner;

    private IPlayer player1;
    private IPlayer player2;
    
    public ColliseumExperimentWorker(IDeckShuffler shuffler, IEnumerable<IPlayer> plys)
    {
        runner = new ExperimentRunner(shuffler);
        this.player1 = plys.First();
        this.player2 = plys.Skip(1).First(); // so much for fancy DI trash
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"pitting {this.player1} and {this.player2}...");

        int counter = 0;
        int tries = 1000000;

        for (int i = 0; i < tries; i++)
        {
            ExperimentResult res = runner.RunSingle(this.player1, this.player2);
            counter += res.AllowFight ? 1 : 0;
        }

        Console.WriteLine($"done, {counter}/{tries} fights.");
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}