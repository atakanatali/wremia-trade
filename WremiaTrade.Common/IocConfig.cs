namespace WremiaTrade.Common;

using Autofac;
using RestSharp;

using Constants = WremiaTrade.Utilities.Constants;
using WremiaTrade.Services.Configuration;
using WremiaTrade.Services.Configuration.Interface;
public static class IocConfig
{
    public static ContainerBuilder RegisterServicesForApplication()
    {
        var builder = new ContainerBuilder();
        
        return builder;
    }
    
    public static ContainerBuilder RegisterServicesForWeb(ContainerBuilder builder)
    {
        return builder;
    }
    
    public static ContainerBuilder RegisterServicesNotifications()
    {
        var builder = new ContainerBuilder();

        // builder.RegisterType<RedisConnectionFactory>().As<IRedisConnectionFactory>().SingleInstance();
        //
        // builder.RegisterType<BackgroundJobWrapper>().As<IBackgroundJobWrapper>().SingleInstance();
        //
        // builder.RegisterType<NotificationHub>().As<INotificationHub>().InstancePerLifetimeScope();

        builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();

        // builder.RegisterType<LifetimeScopeAdapter>().As<ILifetimeScopeAdapter>().InstancePerLifetimeScope();
        //
        // builder.RegisterType<RestSharpProvider>().As<IHttpClientProvider>().InstancePerDependency();

        return builder;
    }
    
    /// <summary>
    /// We have to initialize RedisCacheService depended abstract class
    /// </summary>
    public static void InitializeRedisAbstractionDependedServices(IContainer container)
    {
        // RedisAbstractDependedService.RedisConnectionFactory = container.Resolve<IRedisConnectionFactory>();
        //
        // RedisAbstractDependedService.MemoryCacheService = container.Resolve<IMemoryCacheService>();
    }
}