namespace WremiaTrade.Models.Dtos
{
    using System;

    using WremiaTrade.Models.Entities.Trade;

    public class TradeOrderDto
    {
        public Guid Id { get; set; }

        public string Symbol { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public TradeOrderSide Side { get; set; }

        public decimal? LimitPrice { get; set; }

        public TradeOrderStatus Status { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? ScheduledFor { get; set; }

        public DateTimeOffset? ExecutedAt { get; set; }

        public decimal? ExecutedPrice { get; set; }

        public string? Notes { get; set; }

        public static TradeOrderDto FromEntity(Entities.Trade.TradeOrder order)
        {
            return new TradeOrderDto
            {
                Id = order.Id,
                Symbol = order.Symbol,
                Quantity = order.Quantity,
                Side = order.Side,
                LimitPrice = order.LimitPrice,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                ScheduledFor = order.ScheduledFor,
                ExecutedAt = order.ExecutedAt,
                ExecutedPrice = order.ExecutedPrice,
                Notes = order.Notes,
            };
        }
    }
}
