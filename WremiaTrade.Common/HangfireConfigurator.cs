namespace WremiaTrade.Common
{
    using Autofac;

    using WremiaTrade.ConfigAdapter;

    using Hangfire;

    using WremiaTrade.Utilities;
    public class HangfireConfigurator
    {
        /// <summary>
        /// Configures Hangfire for Redis Storage
        /// Registers Autofac container, so hangfire initiates instances by interfaces,
        /// sets automatic retry to 10 times for failed jobs
        /// </summary>
        /// <param name="container">Papara IOC Container</param>
        public static void ConfigureHangfire(IContainer container)
        {
            GlobalConfiguration.Configuration.UseRedisStorage(ConfigurationAdaptor.Manage.GetValueFromConnectionStrings<string>(Constants.ConnectionStrings.HangfireRedis));

            GlobalConfiguration.Configuration.UseAutofacActivator(container, false);

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 10 });
        }
    }
}