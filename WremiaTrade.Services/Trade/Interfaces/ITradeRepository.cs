namespace WremiaTrade.Services.Trade.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using WremiaTrade.Models.Entities.Trade;

    public interface ITradeRepository
    {
        Task AddAsync(TradeOrder order, CancellationToken cancellationToken = default);

        Task<TradeOrder?> GetAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<TradeOrder>> GetAllAsync(CancellationToken cancellationToken = default);

        Task UpdateAsync(TradeOrder order, CancellationToken cancellationToken = default);
    }
}
