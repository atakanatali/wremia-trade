namespace WremiaTrade.HttpClientProvider.Interfaces
{
    using WremiaTrade.RestSharpClient.Enums;

    /// <summary>
    /// IHttpClient for RestSharp
    /// </summary>
    public interface IHttpClientProvider
    {
        /// <summary>
        /// Enum definition for request serializer
        /// Camel, Normal, LowerCase, SnakeCase
        /// </summary>
        RequestSerializer RequestSerializer { get; set; }

        /// <summary>
        /// External service host address
        /// </summary>
        string ServiceHost { get; set; }

        /// <summary>
        /// Predefined headers values for external service requests
        /// </summary>
        IDictionary<string, string> DefaultHeaders { get; set; }

        /// <summary>
        /// Set or get the timeout of request in seconds. Default timeout is 30
        /// </summary>
        int TimeoutInSeconds { get; set; }

        /// <summary>
        /// Provides delete request and deserialize json response according to given response type
        /// </summary>
        /// <param name="endpoint">Endpoint to append to the ServiceHost</param>
        /// <param name="timeoutInSeconds">Timeout period for the request.</param>
        /// <param name="retryOnFailure">Triggers retry request mechanism on failure</param>
        /// <returns TResponse="object">deserialized response content</returns>
        TResponse DeleteJson<TResponse>(
            string endpoint,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false);

        /// <summary>
        /// Provides delete request and deserialize json response according to given response type
        /// </summary>
        /// <param name="endpoint">Endpoint to append to the ServiceHost</param>
        /// <param name="payload">Data to add to the body of the request in JSON format.</param>
        /// <param name="timeoutInSeconds">Timeout period for the request.</param>
        /// <param name="retryOnFailure">Triggers retry request mechanism on failure</param>
        /// <returns TResponse="object">deserialized response content</returns>
        TResponse DeleteJson<TResponse>(
            string endpoint,
            object payload,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false);

        /// <summary>
        /// Provides delete request and deserialize json response according to given response type
        /// </summary>
        /// <param name="endpoint">Endpoint to append to the ServiceHost</param>
        /// <param name="headers">Request headers in key,value format.</param>
        /// <param name="payload">Data to add to the body of the request in JSON format.</param>
        /// <param name="timeoutInSeconds">Timeout period for the request.</param>
        /// <param name="retryOnFailure">Triggers retry request mechanism on failure</param>
        /// <returns TResponse="object">deserialized response content</returns>
        TResponse DeleteJson<TResponse>(
            string endpoint,
            IDictionary<string, string> headers,
            object payload,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false);

        /// <summary>
        /// Provides get request and deserialize json response according to given response type
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <param name="retryOnFailure">triggers retry request mechanism on failure</param>
        /// <returns TResponse="object">deserialized response content</returns>
        TResponse GetJson<TResponse>(
            string endpoint,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false)
            where TResponse : class;

        /// <summary>
        /// Provides get request and deserialize json response according to given response type
        /// </summary>
        /// <typeparam name="TResponse">ServiceResult.Data type</typeparam>
        /// <param name="endpoint">Endpoint to append to the ServiceHost</param>
        /// <param name="payload">Data to add to the body of the request in JSON format.</param>
        /// <param name="timeoutInSeconds">Timeout period for the request.</param>
        /// <param name="retryOnFailure">Triggers retry request mechanism on failure</param>
        /// <returns TResponse="object">deserialized response content</returns>
        TResponse GetJson<TResponse>(
            string endpoint,
            object payload,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false)
            where TResponse : class;

        /// <summary>
        /// Provides get request and deserialize json response according to given response type
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="headers"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <param name="retryOnFailure">triggers retry request mechanism on failure</param>
        /// <returns TResponse="object">deserialized response content</returns>
        TResponse GetJson<TResponse>(
            string endpoint,
            IDictionary<string, string> headers,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false)
            where TResponse : class;

        /// <summary>
        /// Provides get request and deserialize json response according to given response type
        /// </summary>
        /// <typeparam name="TResponse">ServiceResult.Data type</typeparam>
        /// <param name="endpoint">Endpoint to append to the ServiceHost</param>
        /// <param name="headers">Request headers in key,value format.</param>
        /// <param name="payload">Data to add to the body of the request in JSON format.</param>
        /// <param name="timeoutInSeconds">Timeout period for the request.</param>
        /// <param name="retryOnFailure">Triggers retry request mechanism on failure</param>
        /// <returns TResponse="object">deserialized response content</returns>
        TResponse GetJson<TResponse>(
            string endpoint,
            IDictionary<string, string> headers,
            object payload,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false)
            where TResponse : class;
 
        /// <summary>
        /// Provides get request and deserialize json response according to given response type
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="headers"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <param name="retryOnFailure">triggers retry request mechanism on failure</param>
        /// <param name="expectJsonResult">deserializes types other than json type like img/png</param>
        /// <returns TResponse="object">deserialized response content</returns>
        byte[] DownloadData<TResponse>(
            string endpoint,
            IDictionary<string, string> headers,
            object payload,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false,
            bool expectJsonResult = true)
            where TResponse : class;

        /// <summary>
        /// Provides post request and deserialize json response according to given response type
        /// </summary>
        /// <typeparam name="TResponse">response type</typeparam>
        /// <param name="endPoint">request endpoint</param>
        /// <param name="timeoutInSeconds">request timeout value in seconds</param>
        /// <param name="retryOnFailure">triggers retry request mechanism on failure</param>
        /// <returns TResponse="object">deserialized response content</returns>
        TResponse PostJson<TResponse>(
            string endPoint,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false)
            where TResponse : class;

        /// <summary>
        /// Provides post request and deserialize json response according to given response type
        /// </summary>
        /// <typeparam name="TResponse">response type</typeparam>
        /// <param name="endPoint">request endpoint</param>
        /// <param name="headers">request headers</param>
        /// <param name="timeoutInSeconds">request timeout value in seconds</param>
        /// <param name="retryOnFailure">triggers retry request mechanism on failure</param>
        /// <returns TResponse="object">deserialized response content</returns>
        TResponse PostJson<TResponse>(
            string endPoint,
            IDictionary<string, string> headers,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false)
            where TResponse : class;

        /// <summary>
        /// Provides post request and deserialize json response according to given response type
        /// </summary>
        /// <typeparam name="TResponse">ServiceResult.Data type</typeparam>
        /// <param name="endpoint">Endpoint to append to the ServiceHost</param>
        /// <param name="payload">Data to add to the body of the request in JSON format.</param>
        /// <param name="timeoutInSeconds">Timeout period for the request.</param>
        /// <param name="retryOnFailure">Triggers retry request mechanism on failure</param>
        /// <returns TResponse="object">deserialized response content</returns>
        TResponse PostJson<TResponse>(
            string endpoint,
            object payload,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false)
            where TResponse : class;

        /// <summary>
        /// Provides post request and deserialize json response according to given response type
        /// </summary>
        /// <typeparam name="TResponse">ServiceResult.Data type</typeparam>
        /// <param name="endpoint">Endpoint to append to the ServiceHost</param>
        /// <param name="payload">Data to add to the body of the request in JSON format.</param>
        /// <param name="headers">Request headers in key,value format.</param>
        /// <param name="timeoutInSeconds">Timeout period for the request.</param>
        /// <param name="retryOnFailure">Triggers retry request mechanism on failure</param>
        /// <param name="throwIfUnSuccess">if request status code is not success than throws error</param>
        /// <returns TResponse="object">deserialized response content</returns>
        TResponse PostJson<TResponse>(
            string endpoint,
            object payload,
            IDictionary<string, string> headers,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false,
            bool? throwIfUnSuccess = false)
            where TResponse : class;

        /// <summary>
        /// Provides put request and deserialize json response according to given response type 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="payload"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        TResponse PutJson<TResponse>(
            string endpoint,
            object payload,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false,
            bool? throwIfUnSuccess = false)
            where TResponse : class;

        /// <summary>
        /// Provides put request and deserialize json response according to given response type 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="payload"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <param name="basicAuthUserName"></param>
        /// <param name="basicAuthPassword"></param>
        /// <returns></returns>
        TResponse PutJson<TResponse>(
            string endpoint,
            object payload,
            string basicAuthUserName,
            string basicAuthPassword,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false,
            bool? throwIfUnSuccess = false)
            where TResponse : class;

        /// <summary>
        /// Provides put request and deserialize json response according to given response type 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="endPoint"></param>
        /// <param name="payload"></param>
        /// <param name="headers"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        TResponse PutJson<TResponse>(
            string endpoint,
            object payload,
            IDictionary<string, string> headers,
            int? timeoutInSeconds = null,
            bool retryOnFailure = false,
            bool? throwIfUnSuccess = false)
            where TResponse : class;
    }

}
