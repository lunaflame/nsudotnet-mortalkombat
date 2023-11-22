using Contracts.Interfaces;
using Nsu.MortalKombat.Gods;

namespace HostRunner;

public class ColliseumExperimentWorker : IHostedService
{
	private readonly IPlayer player1;
	private readonly IPlayer player2;
	private readonly ExperimentRunner runner;
	private CancellationToken tok;

	public ColliseumExperimentWorker(IDeckShuffler shuffler, IEnumerable<IPlayer> plys)
	{
		runner = new ExperimentRunner(shuffler);
		player1 = plys.First();
		player2 = plys.Skip(1).First(); // so much for fancy DI trash
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine("start async...");
		tok = cancellationToken;

		Task ret = new Task(() => DoExperiment(1000000));
		ret.Start();

		return Task.CompletedTask; //ret.WaitAsync(cancellationToken);;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	//public int DoExperiment(int times = 1, int cancelPollFreq = 5000)

	private int DoExperiment(int times = 1)
	{
		Console.WriteLine("doing experiment");
		int wins = 0;
		for (int n = 0; n < times; n++)
		{
			if (tok.IsCancellationRequested) return -1;

			ExperimentResult res = runner.RunSingle(player1, player2);
			wins += res.AllowFight ? 1 : 0;
		}

		Console.WriteLine($"Experiment finished: {wins}/{times} fights would occur.");

		return wins;

		/*
		int checkCancelTimes = times / cancelPollFreq;
		int runRemainder = times % cancelPollFreq;
	
		for (int i = 0; i < checkCancelTimes; i++)

		{
		    if (!tok.IsCancellationRequested)
		    {
		        for (int n = 0; n < times; n++)
		        {
		            ExperimentResult res = runner.RunSingle(this.player1, this.player2);
		            wins += res.AllowFight ? 1 : 0;
		        }
		    }
		}
		
		for (int n = 0; n < runRemainder; n++)
		{
		    ExperimentResult res = runner.RunSingle(this.player1, this.player2);
		    wins += res.AllowFight ? 1 : 0;
		}

		return wins;
		*/
	}
}