namespace WremiaTrade.Services.Trade.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using WremiaTrade.Models.Events;

    public interface ITradeEventPublisher
    {
        Task PublishAsync(TradeOrderEvent integrationEvent, CancellationToken cancellationToken);
    }
}
