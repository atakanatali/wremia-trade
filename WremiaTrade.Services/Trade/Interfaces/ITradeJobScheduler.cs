namespace WremiaTrade.Services.Trade.Interfaces
{
    using System;

    public interface ITradeJobScheduler
    {
        string EnqueueExecution(Guid orderId);

        string ScheduleExecution(Guid orderId, TimeSpan delay);

        void EnsureRecurringHealthCheck();
    }
}
