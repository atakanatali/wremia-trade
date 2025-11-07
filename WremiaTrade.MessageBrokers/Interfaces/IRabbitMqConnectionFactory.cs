namespace WremiaTrade.MessageBrokers.Interfaces
{
    using RabbitMQ.Client;

    public interface IRabbitMqConnectionFactory
    {
        IConnection CreateConnection();
    }
}
