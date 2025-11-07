namespace WremiaTrade.MessageBrokers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMessagePublisher
    {
        Task PublishAsync(string routingKey, string payload, CancellationToken cancellationToken = default);
    }
}
