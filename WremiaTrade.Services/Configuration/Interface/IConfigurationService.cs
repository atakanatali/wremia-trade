namespace WremiaTrade.Services.Configuration.Interface
{
    using System;
    using System.Configuration;
    using Environment = WremiaTrade.Utilities.Enums.Environments;

    public interface IConfigurationService
    {
        /// <summary>
        /// Reads application settings and returns it on result.
        /// If configuration not exists than returns false. (If throw exception parameter is true than throws 
        /// </summary>
        /// <param name="configurationKey">Key for configuration setting</param>
        /// <param name="result">Result for the configuration</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        bool ReadConfiguration(string configurationKey, out int result, bool throwException = true);

        /// <summary>
        /// Reads application settings and returns it on result.
        /// If configuration not exists than returns false. If throw exception parameter is true than throws 
        /// </summary>
        /// <param name="configurationKey">Key for configuration setting</param>
        /// <param name="result">Result for the configuration</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        bool ReadConfiguration(string configurationKey, out bool result, bool throwException = true);

        /// <summary>
        /// Reads application settings and returns it on result.
        /// If configuration not exists than returns false. 
        /// If throw exception parameter is true than throws ConfigurationErrorsException
        /// </summary>
        /// <param name="configurationKey">Key for configuration setting</param>
        /// <param name="result">Result for the configuration</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        bool ReadConfiguration(string configurationKey, out string result, bool throwException = true);

        /// <summary>
        /// Reads application settings and returns it on result.
        /// If configuration not exists than returns false. (If throw exception parameter is true than throws 
        /// </summary>
        /// <param name="configurationKey">Key for configuration setting</param>
        /// <param name="result">Result for the configuration</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        bool ReadConfiguration(string configurationKey, out decimal result, bool throwException = true);

        /// <summary>
        /// Reads application settings and returns it on result.
        /// If configuration not exists than returns false. (If throw exception parameter is true than throws 
        /// </summary>
        /// <param name="configurationKey">Key for configuration setting</param>
        /// <param name="result">Result for the configuration</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <returns></returns>
        bool ReadConfiguration(string configurationKey, out TimeSpan result, bool throwException = true);

        /// <summary>
        /// Reads connection string settings and returns it on result.
        /// If connection string configuration not exists than returns false.
        /// If throw exception parameter is true than throws ConfigurationErrorsException
        /// </summary>
        /// <param name="configurationKey">Key for connection string configuration setting</param>
        /// <param name="result">Result for the connection string</param>
        /// <param name="throwException">Optional parameter for throwing exception if setting not exists</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        bool ReadConnectionStringConfiguration(string configurationKey, out string result, bool throwException = true);


        /// <summary>
        /// Gets the environment from the configuration
        /// Configuration key LogEnvironment should be set as LOCAL/TEST/PRODUCTION
        /// </summary>
        Environment GetEnvironment();
    }
}
