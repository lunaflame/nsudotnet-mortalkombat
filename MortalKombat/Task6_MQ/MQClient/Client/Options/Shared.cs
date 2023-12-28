using MassTransit;
using MQStart.Client;
using Nsu.MortalKombat.DeckShufflers;

namespace GodClient;

public partial class Options
{
    private PlayerExperimentQuerier querier;

    public Options(PlayerExperimentQuerier querier)
    {
        this.querier = querier;
    }
    
    private static DeckShuffler shuffler = new DeckShuffler();
}