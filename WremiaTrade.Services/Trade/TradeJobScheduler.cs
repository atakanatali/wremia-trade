namespace WremiaTrade.Services.Trade
{
    using System;

    using Hangfire;

    using Microsoft.Extensions.Logging;

    using WremiaTrade.Services.Trade.Interfaces;
    using WremiaTrade.Services.Trade.Jobs;

    public class TradeJobScheduler : ITradeJobScheduler
    {
        private const string HealthCheckJobId = "trading-health-check";
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IRecurringJobManager recurringJobManager;
        private readonly ILogger<TradeJobScheduler> logger;

        public TradeJobScheduler(
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager,
            ILogger<TradeJobScheduler> logger)
        {
            this.backgroundJobClient = backgroundJobClient;
            this.recurringJobManager = recurringJobManager;
            this.logger = logger;
        }

        public string EnqueueExecution(Guid orderId)
        {
            var jobId = backgroundJobClient.Enqueue<TradeExecutionJob>(job => job.ExecuteAsync(orderId, default));
            logger.LogInformation("Hangfire job {JobId} enqueued for order {OrderId}", jobId, orderId);
            return jobId;
        }

        public string ScheduleExecution(Guid orderId, TimeSpan delay)
        {
            var jobId = backgroundJobClient.Schedule<TradeExecutionJob>(job => job.ExecuteAsync(orderId, default), delay);
            logger.LogInformation("Hangfire job {JobId} scheduled for order {OrderId} with delay {Delay}", jobId, orderId, delay);
            return jobId;
        }

        public void EnsureRecurringHealthCheck()
        {
            recurringJobManager.AddOrUpdate<TradeExecutionJob>(
                HealthCheckJobId,
                job => job.PerformHealthCheckAsync(),
                Cron.Hourly);
            logger.LogDebug("Recurring trading health-check job ensured");
        }
    }
}
