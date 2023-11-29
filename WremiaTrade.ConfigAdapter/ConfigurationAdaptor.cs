namespace WremiaTrade.ConfigAdapter;

using Microsoft.Extensions.Configuration;
public class ConfigurationAdaptor
{
    public static class Manage
    {
        public static Dictionary<string, string> ConnectionStrings { get; internal set; } = new Dictionary<string, string>();

        public static Dictionary<string, string> AppSettings { get; internal set; } = new Dictionary<string, string>();

        public static void Init(IConfiguration configuration) 
        {
            AppSettings = configuration
                .GetSection("AppSettings")
                .GetChildren()
                .ToDictionary(x => x.Key, y => y.Value)!;

            ConnectionStrings = configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(x => x.Key, y => y.Value)!;
        }

        public static TValue GetValueFromAppSettings<TValue>(string key, bool throwException = true)
            where TValue : IConvertible
        {
            if (!AppSettings.ContainsKey(key) && throwException)
            {
                throw new InvalidOperationException($"{key} not found in AppSettings");
            }

            return (TValue)Convert.ChangeType(AppSettings[key], typeof(TValue));
        }

        public static TValue GetValueFromConnectionStrings<TValue>(string key, bool throwException = true)
            where TValue : IConvertible
        {
            if (!ConnectionStrings.ContainsKey(key) && throwException)
            {
                throw new InvalidOperationException($"{key} not found in ConnectionStrings");
            }

            return (TValue)Convert.ChangeType(ConnectionStrings[key], typeof(TValue));
        }
    }
}