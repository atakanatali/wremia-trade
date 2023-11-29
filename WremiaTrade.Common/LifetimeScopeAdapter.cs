namespace WremiaTrade.Common
{
    using Autofac;

    using WremiaTrade.Services.Interface;
    
    /// <summary>
    /// Runtimeda current IOC container'a register edilmiş bir servisi 
    /// geçerli olan scope çerçevesinde resolve eder.
    /// </summary>
    public class LifetimeScopeAdapter : ILifetimeScopeAdapter
    {
        private readonly ILifetimeScope _scope;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public LifetimeScopeAdapter(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public TService Resolve<TService>()
        {
            return _scope.Resolve<TService>();
        }

        /// <summary>
        /// IOC Container' a isim ile kaydedilmiş servisleri verilen isme ile resolve eder.
        /// </summary>
        public TService ResolveNamed<TService>(string serviceName)
        {
            bool isRegistered = _scope.IsRegisteredWithName<TService>(serviceName);

            if (!isRegistered)
            {
                return default(TService);
            }

            return _scope.ResolveNamed<TService>(serviceName);
        }
    }
}