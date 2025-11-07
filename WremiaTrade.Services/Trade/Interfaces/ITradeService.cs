namespace WremiaTrade.Services.Trade.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using WremiaTrade.Models.Dtos;
    using WremiaTrade.Services.Abstraction;

    public interface ITradeService
    {
        Task<ServiceResult<TradeOrderDto>> CreateOrderAsync(TradeOrderRequest request, CancellationToken cancellationToken = default);

        Task<ServiceResult<IEnumerable<TradeOrderDto>>> GetOrdersAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<TradeOrderDto>> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}
