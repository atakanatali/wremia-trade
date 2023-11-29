namespace WremiaTrade.Services.Interface
{
    public interface ILifetimeScopeAdapter
    {
        /// <summary>
        /// Aynı interface'i implement eden birden fazla sınıf içinden hangisinin
        /// kullanılacağı çalışma zamanında belirleniyor ise verilen isimle resolve etmemize yarar.
        /// 
        /// Örnek:
        /// JetSmsRefName app settings'de Jet olarak veriliyor, bu sınıf bu isimle register oluyor.
        /// Sonrasında app settings'de PrimarySmsProvider=Jet yapıp, _container.ResolveNamed ISmsProvider ("Jet") dersek
        /// container bize JetSmsProvider instance'ı döner.
        /// </summary>
        TService ResolveNamed<TService>(string serviceName);

        /// <summary>
        /// Resolves service with in current lifetime scope
        /// </summary>
        TService Resolve<TService>();
    }
}