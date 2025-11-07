namespace WremiaTrade.Models.Entities.Trade
{
    using System;

    public class TradeOrder
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

        public TradeOrder Clone()
        {
            return (TradeOrder)MemberwiseClone();
        }
    }
}
