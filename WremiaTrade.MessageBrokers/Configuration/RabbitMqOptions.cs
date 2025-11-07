namespace WremiaTrade.MessageBrokers.Configuration
{
    public class RabbitMqOptions
    {
        public string HostName { get; set; } = "localhost";

        public int Port { get; set; } = 5672;

        public string VirtualHost { get; set; } = "/";

        public string UserName { get; set; } = "guest";

        public string Password { get; set; } = "guest";

        public string Exchange { get; set; } = "wremia.trade";

        public string Queue { get; set; } = "wremia.trade.orders";

        public string RoutingKey { get; set; } = "trade.order";

        public bool Durable { get; set; } = true;

        public bool AutoDelete { get; set; }

        public bool Exclusive { get; set; }
    }
}
