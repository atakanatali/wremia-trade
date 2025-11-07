namespace WremiaTrade.Services.Trade
{
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using WremiaTrade.MessageBrokers.Interfaces;
    using WremiaTrade.Models.Events;
    using WremiaTrade.Services.Trade.Interfaces;

    public class TradeEventPublisher : ITradeEventPublisher
    {
        private const string RoutingKey = "trade.order";
        private readonly IMessagePublisher messagePublisher;

        public TradeEventPublisher(IMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        public Task PublishAsync(TradeOrderEvent integrationEvent, CancellationToken cancellationToken)
        {
            var payload = JsonConvert.SerializeObject(integrationEvent);
            return messagePublisher.PublishAsync(RoutingKey, payload, cancellationToken);
        }
    }
}
