using ColliseumPlayers;
using Contracts;

ExperimentRunner runner = new ExperimentRunner();
IPlayer p1 = new Elon();
IPlayer p2 = new Zucc();

Console.WriteLine("generating...");

int counter = 0;
int tries = 1000000;

for (int i = 0; i < tries; i++)
{
    ExperimentResult res = runner.RunSingle(p1, p2);
    counter += res.AllowFight ? 1 : 0;
}

Console.WriteLine($"done, {counter}/{tries} fights.");