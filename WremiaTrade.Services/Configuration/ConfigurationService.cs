namespace WremiaTrade.Services.Configuration
{
    using System;
    using System.Configuration;
    using System.Globalization;

    using Environment = Utilities.Enums.Environments;

    using Interface;
    using Utilities;

    using ConfigAdapter;
    
    /// <summary>
    /// Service class for reading app and connection string settings
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        /// <summary>
        /// Reads application settings and returns it on result.
        /// If configuration not exists than returns false. (If throw exception parameter is true than throws 
        /// </summary>
        /// <param name="configurationKey">Key for configuration setting</param>
        /// <param name="result">Result for the configuration</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public bool ReadConfiguration(string configurationKey, out int result, bool throwException = true)
        {
            bool configurationExists = int.TryParse(ConfigurationAdaptor.Manage.GetValueFromAppSettings<string>(configurationKey), out result);

            if (throwException && !configurationExists)
            {
                throw new ConfigurationErrorsException($"{configurationKey} degeri config dosyasinda ekli olmali");
            }

            return configurationExists;
        }

        /// <summary>
        /// Reads application settings and returns it on result.
        /// If configuration not exists than returns false. If throw exception parameter is true than throws 
        /// </summary>
        /// <param name="configurationKey">Key for configuration setting</param>
        /// <param name="result">Result for the configuration</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public bool ReadConfiguration(string configurationKey, out bool result, bool throwException = true)
        {
            bool configurationExists = bool.TryParse(ConfigurationAdaptor.Manage.GetValueFromAppSettings<string>(configurationKey), out result);

            if (throwException && !configurationExists)
            {
                throw new ConfigurationErrorsException($"{configurationKey} degeri config dosyasinda ekli olmali");
            }

            return configurationExists;
        }

        /// <summary>
        /// Reads application settings and returns it on result.
        /// If configuration not exists than returns false. 
        /// If throw exception parameter is true than throws ConfigurationErrorsException
        /// </summary>
        /// <param name="configurationKey">Key for configuration setting</param>
        /// <param name="result">Result for the configuration</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public bool ReadConfiguration(string configurationKey, out string result, bool throwException = true)
        {
            result = ConfigurationAdaptor.Manage.GetValueFromAppSettings<string>(configurationKey);

            if (throwException && string.IsNullOrWhiteSpace(result))
            {
                throw new ConfigurationErrorsException($"{configurationKey} degeri config dosyasinda ekli olmali");
            }

            return !string.IsNullOrWhiteSpace(result);
        }

        /// <summary>
        /// Reads application settings and returns it on result.
        /// If configuration not exists than returns false. (If throw exception parameter is true than throws 
        /// </summary>
        /// <param name="configurationKey">Key for configuration setting</param>
        /// <param name="result">Result for the configuration</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public bool ReadConfiguration(string configurationKey, out decimal result, bool throwException = true)
        {
            bool configurationExists = decimal.TryParse(ConfigurationAdaptor.Manage.GetValueFromAppSettings<string>(configurationKey), NumberStyles.Number, CultureInfo.InvariantCulture, out result);

            if (throwException && !configurationExists)
            {
                throw new ConfigurationErrorsException($"{configurationKey} degeri config dosyasinda ekli olmali");
            }

            return configurationExists;
        }

        /// <summary>
        /// Reads application settings and returns it on result.
        /// If configuration not exists than returns false. (If throw exception parameter is true than throws 
        /// </summary>
        /// <param name="configurationKey">Key for configuration setting</param>
        /// <param name="result">Result for the configuration</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public bool ReadConfiguration(string configurationKey, out TimeSpan result, bool throwException = true)
        {
            bool configurationExists = TimeSpan.TryParse(ConfigurationAdaptor.Manage.GetValueFromAppSettings<string>(configurationKey), CultureInfo.InvariantCulture, out result);

            if (throwException && !configurationExists)
            {
                throw new ConfigurationErrorsException($"{configurationKey} degeri config dosyasinda ekli olmali");
            }

            return configurationExists;
        }

        /// <summary>
        /// Reads connection string settings and returns it on result.
        /// If connection string configuration not exists than returns false.
        /// If throw exception parameter is true than throws ConfigurationErrorsException
        /// </summary>
        /// <param name="configurationKey">Key for connection string configuration setting</param>
        /// <param name="result">Result for the connection string</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public bool ReadConnectionStringConfiguration(string configurationKey, out string result, bool throwException = true)
        {
            result = ConfigurationAdaptor.Manage.GetValueFromAppSettings<string>($"ConnectionStrings: {configurationKey}");

            if (throwException && string.IsNullOrWhiteSpace(result))
            {
                throw new ConfigurationErrorsException($"{configurationKey} degeri config dosyasinda connection string olarak ekli olmali");
            }

            return string.IsNullOrWhiteSpace(result);
        }

        public Environment GetEnvironment()
        {
            ReadConfiguration("LogEnvironment", out string environment);

            return EnumHelper.ParseEnum<Environment>(environment);
        }
    }
}
