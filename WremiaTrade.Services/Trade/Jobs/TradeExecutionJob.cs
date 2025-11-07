namespace WremiaTrade.Services.Trade.Jobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using WremiaTrade.Models.Entities.Trade;
    using WremiaTrade.Models.Events;
    using WremiaTrade.Services.Trade.Interfaces;

    public class TradeExecutionJob
    {
        private readonly ITradeRepository repository;
        private readonly ITradeEventPublisher eventPublisher;
        private readonly ILogger<TradeExecutionJob> logger;

        public TradeExecutionJob(
            ITradeRepository repository,
            ITradeEventPublisher eventPublisher,
            ILogger<TradeExecutionJob> logger)
        {
            this.repository = repository;
            this.eventPublisher = eventPublisher;
            this.logger = logger;
        }

        public async Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var order = await repository.GetAsync(orderId, cancellationToken);
            if (order == null)
            {
                logger.LogWarning("Trade order {OrderId} not found during execution", orderId);
                return;
            }

            if (order.Status == TradeOrderStatus.Executed)
            {
                logger.LogInformation("Trade order {OrderId} already executed", orderId);
                return;
            }

            order.Status = TradeOrderStatus.Executing;
            await repository.UpdateAsync(order, cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            order.Status = TradeOrderStatus.Executed;
            order.ExecutedAt = DateTimeOffset.UtcNow;
            order.ExecutedPrice = order.LimitPrice ?? Math.Round(order.Quantity * 1.05m, 4);
            await repository.UpdateAsync(order, cancellationToken);

            var integrationEvent = TradeOrderEvent.Executed(order);
            await eventPublisher.PublishAsync(integrationEvent, cancellationToken);

            logger.LogInformation("Trade order {OrderId} executed at {ExecutedPrice}", order.Id, order.ExecutedPrice);
        }

        public Task PerformHealthCheckAsync()
        {
            logger.LogTrace("Trading health check executed at {Now}", DateTimeOffset.UtcNow);
            return Task.CompletedTask;
        }
    }
}
