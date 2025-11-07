namespace WremiaTrade.Models.Entities.Trade
{
    public enum TradeOrderStatus
    {
        Pending = 0,
        Scheduled = 1,
        Executing = 2,
        Executed = 3,
        Failed = 4,
        Cancelled = 5,
    }
}
