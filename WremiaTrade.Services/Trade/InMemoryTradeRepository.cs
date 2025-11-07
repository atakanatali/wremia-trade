namespace WremiaTrade.Services.Trade
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using WremiaTrade.Models.Entities.Trade;
    using WremiaTrade.Services.Trade.Interfaces;

    public class InMemoryTradeRepository : ITradeRepository
    {
        private readonly ConcurrentDictionary<Guid, TradeOrder> storage = new();

        public Task AddAsync(TradeOrder order, CancellationToken cancellationToken = default)
        {
            storage[order.Id] = order.Clone();
            return Task.CompletedTask;
        }

        public Task<TradeOrder?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            storage.TryGetValue(id, out var order);
            return Task.FromResult(order?.Clone());
        }

        public Task<IReadOnlyCollection<TradeOrder>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<TradeOrder> result = storage.Values.Select(x => x.Clone()).ToList();
            return Task.FromResult(result);
        }

        public Task UpdateAsync(TradeOrder order, CancellationToken cancellationToken = default)
        {
            storage[order.Id] = order.Clone();
            return Task.CompletedTask;
        }
    }
}
