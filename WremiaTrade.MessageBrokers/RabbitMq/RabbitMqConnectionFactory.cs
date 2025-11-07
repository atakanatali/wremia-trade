namespace WremiaTrade.MessageBrokers.RabbitMq
{
    using System;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using RabbitMQ.Client;

    using WremiaTrade.MessageBrokers.Configuration;
    using WremiaTrade.MessageBrokers.Interfaces;

    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory, IDisposable
    {
        private readonly RabbitMqOptions options;
        private readonly ILogger<RabbitMqConnectionFactory> logger;
        private IConnection? connection;
        private bool disposed;

        public RabbitMqConnectionFactory(IOptions<RabbitMqOptions> options, ILogger<RabbitMqConnectionFactory> logger)
        {
            this.options = options.Value;
            this.logger = logger;
        }

        public IConnection CreateConnection()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMqConnectionFactory));
            }

            if (connection != null && connection.IsOpen)
            {
                return connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                VirtualHost = options.VirtualHost,
                UserName = options.UserName,
                Password = options.Password,
                DispatchConsumersAsync = true,
            };

            connection = factory.CreateConnection();
            logger.LogInformation("RabbitMQ connection established to {Host}:{Port}", options.HostName, options.Port);
            return connection;
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
                connection?.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error disposing RabbitMQ connection");
            }
        }
    }
}
