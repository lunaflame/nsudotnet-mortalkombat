namespace Company.Consumers
{
    using MassTransit;

    public class MQStartConsumerDefinition :
        ConsumerDefinition<MQStartConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<MQStartConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}