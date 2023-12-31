using Contracts.Interfaces;
using Nsu.MortalKombat.DeckShufflers;
using Nsu.MortalKombat.Players;

namespace HostRunner;

internal class Program
{
	public static void Main(string[] args)
	{
		IHostBuilder bld = CreateHostBuilder(args);
		bld.Build().Run();
	}

	public static IHostBuilder CreateHostBuilder(string[] args)
	{
		return Host.CreateDefaultBuilder(args)
			.ConfigureServices((hostContext, services) =>
			{
				services.AddHostedService<ColliseumExperimentWorker>();
				services.AddScoped<IDeckShuffler, DeckShuffler>();

				services.AddTransient<IPlayer, Zucc>();
				services.AddTransient<IPlayer, Elon>();
			});
	}
}