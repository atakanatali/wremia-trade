namespace WremiaTrade.Models.Events
{
    using System;

    using WremiaTrade.Models.Dtos;
    using WremiaTrade.Models.Entities.Trade;

    public class TradeOrderEvent
    {
        public TradeOrderEvent(string eventType, TradeOrderDto order)
        {
            EventType = eventType;
            Order = order;
            OccurredAt = DateTimeOffset.UtcNow;
        }

        public string EventType { get; }

        public TradeOrderDto Order { get; }

        public DateTimeOffset OccurredAt { get; }

        public static TradeOrderEvent Accepted(TradeOrder order)
        {
            return new TradeOrderEvent("OrderAccepted", TradeOrderDto.FromEntity(order));
        }

        public static TradeOrderEvent Executed(TradeOrder order)
        {
            return new TradeOrderEvent("OrderExecuted", TradeOrderDto.FromEntity(order));
        }
    }
}
