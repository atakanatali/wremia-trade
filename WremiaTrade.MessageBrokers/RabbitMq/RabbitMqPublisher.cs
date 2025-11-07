namespace WremiaTrade.MessageBrokers.RabbitMq
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using RabbitMQ.Client;

    using WremiaTrade.MessageBrokers.Configuration;
    using WremiaTrade.MessageBrokers.Interfaces;

    public class RabbitMqPublisher : IMessagePublisher, IDisposable
    {
        private readonly IRabbitMqConnectionFactory connectionFactory;
        private readonly RabbitMqOptions options;
        private readonly ILogger<RabbitMqPublisher> logger;
        private IModel? channel;
        private bool disposed;

        public RabbitMqPublisher(
            IRabbitMqConnectionFactory connectionFactory,
            IOptions<RabbitMqOptions> options,
            ILogger<RabbitMqPublisher> logger)
        {
            this.connectionFactory = connectionFactory;
            this.options = options.Value;
            this.logger = logger;
        }

        public Task PublishAsync(string routingKey, string payload, CancellationToken cancellationToken = default)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMqPublisher));
            }

            var model = EnsureChannel();

            var body = Encoding.UTF8.GetBytes(payload);

            var properties = model.CreateBasicProperties();
            properties.Persistent = options.Durable;

            model.BasicPublish(exchange: options.Exchange, routingKey: routingKey, basicProperties: properties, body: body);
            logger.LogInformation("Message published to {Exchange} with routing key {RoutingKey}", options.Exchange, routingKey);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            try
            {
                channel?.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error disposing RabbitMQ channel");
            }
        }

        private IModel EnsureChannel()
        {
            if (channel != null && channel.IsOpen)
            {
                return channel;
            }

            var connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(options.Exchange, ExchangeType.Topic, durable: options.Durable, autoDelete: options.AutoDelete);
            channel.QueueDeclare(options.Queue, durable: options.Durable, exclusive: options.Exclusive, autoDelete: options.AutoDelete);
            channel.QueueBind(options.Queue, options.Exchange, options.RoutingKey);

            return channel;
        }
    }
}
