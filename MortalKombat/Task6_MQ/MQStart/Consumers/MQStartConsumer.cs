namespace Company.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Contracts;

    public class MQStartConsumer :
        IConsumer<MQStart>
    {
        public Task Consume(ConsumeContext<MQStart> context)
        {
            return Task.CompletedTask;
        }
    }
}