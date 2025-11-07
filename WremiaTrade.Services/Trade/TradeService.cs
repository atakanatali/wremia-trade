namespace WremiaTrade.Services.Trade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using WremiaTrade.Models.Dtos;
    using WremiaTrade.Models.Entities.Trade;
    using WremiaTrade.Models.Events;
    using WremiaTrade.Services.Abstraction;
    using WremiaTrade.Services.Trade.Interfaces;

    public class TradeService : ITradeService
    {
        private readonly ITradeRepository repository;
        private readonly ITradeJobScheduler jobScheduler;
        private readonly ITradeEventPublisher eventPublisher;
        private readonly ILogger<TradeService> logger;

        public TradeService(
            ITradeRepository repository,
            ITradeJobScheduler jobScheduler,
            ITradeEventPublisher eventPublisher,
            ILogger<TradeService> logger)
        {
            this.repository = repository;
            this.jobScheduler = jobScheduler;
            this.eventPublisher = eventPublisher;
            this.logger = logger;
        }

        public async Task<ServiceResult<TradeOrderDto>> CreateOrderAsync(TradeOrderRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                return ServiceResult.Failed<TradeOrderDto>(ServiceError.DefaultError);
            }

            if (string.IsNullOrWhiteSpace(request.Symbol))
            {
                return ServiceResult.Failed<TradeOrderDto>(ServiceError.CustomMessage("Symbol alanı boş olamaz"));
            }

            if (request.Quantity <= 0)
            {
                return ServiceResult.Failed<TradeOrderDto>(ServiceError.CustomMessage("Miktar sıfırdan büyük olmalı"));
            }

            var now = DateTimeOffset.UtcNow;
            var order = new TradeOrder
            {
                Id = Guid.NewGuid(),
                Symbol = request.Symbol.ToUpperInvariant(),
                Quantity = request.Quantity,
                Side = request.Side,
                LimitPrice = request.LimitPrice,
                Status = TradeOrderStatus.Pending,
                CreatedAt = now,
                Notes = request.Notes,
            };

            if (request.ExecuteAfter.HasValue && request.ExecuteAfter.Value > TimeSpan.Zero)
            {
                order.Status = TradeOrderStatus.Scheduled;
                order.ScheduledFor = now.Add(request.ExecuteAfter.Value);
            }

            await repository.AddAsync(order, cancellationToken);

            await eventPublisher.PublishAsync(TradeOrderEvent.Accepted(order), cancellationToken);

            jobScheduler.EnsureRecurringHealthCheck();

            if (order.Status == TradeOrderStatus.Scheduled && request.ExecuteAfter.HasValue)
            {
                jobScheduler.ScheduleExecution(order.Id, request.ExecuteAfter.Value);
            }
            else
            {
                jobScheduler.EnqueueExecution(order.Id);
            }

            logger.LogInformation("Trade order {OrderId} created for {Symbol}", order.Id, order.Symbol);

            return ServiceResult.Success(TradeOrderDto.FromEntity(order));
        }

        public async Task<ServiceResult<IEnumerable<TradeOrderDto>>> GetOrdersAsync(CancellationToken cancellationToken = default)
        {
            var orders = await repository.GetAllAsync(cancellationToken);
            var data = orders.Select(TradeOrderDto.FromEntity).ToList();
            return ServiceResult.Success<IEnumerable<TradeOrderDto>>(data);
        }

        public async Task<ServiceResult<TradeOrderDto>> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await repository.GetAsync(orderId, cancellationToken);

            if (order == null)
            {
                return ServiceResult.Failed<TradeOrderDto>(ServiceError.CustomMessage("İşlem bulunamadı"));
            }

            return ServiceResult.Success(TradeOrderDto.FromEntity(order));
        }

    }
}
