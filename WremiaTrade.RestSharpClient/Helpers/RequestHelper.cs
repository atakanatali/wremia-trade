namespace WremiaTrade.RestSharpClient.Helpers
{
    using WremiaTrade.RestSharpClient.Configuration;
    using WremiaTrade.RestSharpClient.Serializers;
    using RestSharp;

    public class RequestHelper
    {
        #region --- Post ---

        /// <summary>
        /// Method to make it easy to post json to given endpoint.
        /// </summary>
        public static IRestResponse PostJson(string requestUrl, dynamic payload, int timeOutSeconds)
        {
            return PostJson(requestUrl, payload, timeOutSeconds, null);
        }

        /// <summary>
        /// Method to make it easy to post json to given endpoint using specified proxy.
        /// </summary>
        public static IRestResponse PostJson(string requestUrl, dynamic payload, int timeOutInSeconds, Uri proxyUri)
        {
            return RequestJson(
                requestUrl,
                payload,
                null,
                timeOutInSeconds,
                null,
                null,
                Method.POST,
                false,
                proxyUri);
        }

        /// <summary>
        /// Method to make it easy to post json to given endpoint that requires ApiKey auth.
        /// </summary>
        public static IRestResponse PostJson(string requestUrl, dynamic payload, int timeOutInSeconds, string apiKey)
        {
            return RequestJson(
                requestUrl,
                payload,
                new Dictionary<string, string> { { "ApiKey", apiKey } },
                timeOutInSeconds,
                null,
                null,
                Method.POST);
        }

        /// <summary>
        /// Method to make it easy to post json to given endpoint.
        /// </summary>
        public static IRestResponse PostJson(string requestUrl, IDictionary<string, string> headers, int timeOutInSeconds, bool retryOnFailure = false)
        {
            return RequestJson(
                requestUrl,
                null,
                headers,
                timeOutInSeconds,
                null,
                null,
                Method.POST,
                retryOnFailure);
        }

        /// <summary>
        /// Method to make it easy to post json to given endpoint.
        /// </summary>
        public static IRestResponse PostJson(string requestUrl, dynamic payload, IDictionary<string, string> headers, int timeOutInSeconds, bool retryOnFailure = false)
        {
            return RequestJson(
                requestUrl,
                payload,
                headers,
                timeOutInSeconds,
                null,
                null,
                Method.POST,
                retryOnFailure);
        }

        /// <summary>
        /// Method to make it easy to post to given endpoint that requires basic auth.
        /// </summary>
        public static IRestResponse PostJson(string requestUrl, dynamic payload, int timeOutSeconds, string basicAuthUserName, string basicAuthPassword, bool retryOnFailure = false)
        {
            IRestResponse response = RequestJson(
                requestUrl,
                payload,
                null,
                timeOutSeconds,
                basicAuthUserName,
                basicAuthPassword,
                Method.POST,
                retryOnFailure);

            return response;
        }

        /// <summary>
        /// Method to make it easy to post encoded form to given endpoint.
        /// </summary>
        public static IRestResponse PostUrlEncodedForm(string requestUrl, dynamic payload, IDictionary<string, string> headers, int timeOutInSeconds, bool retryOnFailure = false)
        {
            return RequestUrlEncodedForm(
                requestUrl,
                payload,
                headers,
                timeOutInSeconds,
                null,
                null,
                Method.POST,
                retryOnFailure);
        }

        #endregion

        #region --- Get ---

        /// <summary>
        /// Method to make it easy to post to given endpoint that requires basic auth.
        /// </summary>
        public static IRestResponse GetJson(string requestUrl, int timeOutInSeconds, string basicAuthUserName, string basicAuthPassword)
        {
            return RequestJson(
                requestUrl,
                null,
                null,
                timeOutInSeconds,
                basicAuthUserName,
                basicAuthPassword,
                Method.GET);
        }

        /// <summary>
        /// Method to make it easy to get to given endpoint that requires .
        /// </summary>
        public static IRestResponse GetJson(string requestUrl, IDictionary<string, string> headers, int timeOutInSeconds, bool retryOnFailure = false, Uri proxyAddress = null)
        {
            return RequestJson(
                requestUrl,
                null,
                headers,
                timeOutInSeconds,
                null,
                null,
                Method.GET,
                retryOnFailure,
                proxyAddress);
        }

        /// <summary>
        /// Method to make it easy to get to given endpoint that requires .
        /// </summary>
        public static IRestResponse GetJson(string requestUrl, int timeOutInSeconds, bool retryOnFailure = false)
        {
            return RequestJson(
                requestUrl,
                null,
                null,
                timeOutInSeconds,
                null,
                null,
                Method.GET,
                retryOnFailure);
        }

        /// <summary>
        /// Method to make it easy to get to given endpoint that requires .
        /// </summary>
        public static IRestResponse GetJson(string requestUrl, IDictionary<string, string> headers, dynamic payload, int timeOutInSeconds, bool retryOnFailure = false)
        {
            return RequestJson(
                requestUrl,
                payload,
                headers,
                timeOutInSeconds,
                null,
                null,
                Method.GET,
                retryOnFailure);
        }

        #endregion


        #region --- Delete ---

        /// <summary>
        /// Method to make it easy to post to given endpoint that requires basic auth.
        /// </summary>
        public static IRestResponse DeleteJson(string requestUrl, int timeOutInSeconds, string basicAuthUserName, string basicAuthPassword)
        {
            return RequestJson(
                requestUrl,
                null,
                null,
                timeOutInSeconds,
                basicAuthUserName,
                basicAuthPassword,
                Method.DELETE);
        }

        /// <summary>
        /// Method to make it easy to get to given endpoint that requires .
        /// </summary>
        public static IRestResponse DeleteJson(string requestUrl, IDictionary<string, string> headers, int timeOutInSeconds, bool retryOnFailure = false)
        {
            return RequestJson(
                requestUrl,
                null,
                headers,
                timeOutInSeconds,
                null,
                null,
                Method.DELETE,
                retryOnFailure);
        }

        /// <summary>
        /// Method to make it easy to get to given endpoint that requires .
        /// </summary>
        public static IRestResponse DeleteJson(string requestUrl, IDictionary<string, string> headers, dynamic payload, int timeOutInSeconds, bool retryOnFailure = false)
        {
            return RequestJson(
                requestUrl,
                payload,
                headers,
                timeOutInSeconds,
                null,
                null,
                Method.DELETE,
                retryOnFailure);
        }

        #endregion

        #region --- Put ---

        /// <summary>
        /// Method to make it easy to put to given endpoint that requires basic auth.
        /// </summary>
        public static IRestResponse PutJson(string requestUrl, dynamic payload, int timeOutSeconds, string basicAuthUserName, string basicAuthPassword)
        {
            IRestResponse response = RequestJson(
                requestUrl,
                payload,
                null,
                timeOutSeconds,
                basicAuthUserName,
                basicAuthPassword,
                Method.PUT);

            return response;
        }

        public static IRestResponse PutJson(string requestUrl, dynamic payload, int timeOutSeconds, IDictionary<string, string> headers = null)
        {
            return RequestJson(requestUrl, payload, headers, timeOutSeconds, null, null, Method.PUT);
        }

        #endregion

        #region --- Private Methods ---

        /// <summary>
        /// Method to make it easy to post,put,delete to given endpoint that requires basic auth.
        /// </summary>
        private static IRestResponse RequestJson(
            string requestUrl,
            dynamic payload,
            IDictionary<string, string> headers,
            int timeOutInSeconds,
            string basicAuthUserName,
            string basicAuthPassword,
            Method method,
            bool retryOnFailure = false,
            Uri proxyAddress = null)
        {
            return RestRequester(
                requestUrl,
                payload,
                headers,
                timeOutInSeconds,
                basicAuthUserName,
                basicAuthPassword,
                method,
                DataFormat.Json,
                retryOnFailure,
                proxyAddress);
        }

        /// <summary>
        /// Method to make it easy to post,put,delete to given endpoint that requires basic auth.
        /// </summary>
        private static IRestResponse RequestUrlEncodedForm(
            string requestUrl,
            dynamic payload,
            IDictionary<string, string> headers,
            int timeOutInSeconds,
            string basicAuthUserName,
            string basicAuthPassword,
            Method method,
            bool retryOnFailure = false,
            Uri proxyAddress = null)
        {
            return RestRequester(
                requestUrl,
                payload,
                headers,
                timeOutInSeconds,
                basicAuthUserName,
                basicAuthPassword,
                method,
                DataFormat.None,
                retryOnFailure,
                proxyAddress);
        }

        /// <summary>
        /// Performs rest request
        /// </summary>
        private static IRestResponse RestRequester(
            string requestUrl,
            dynamic payload,
            IDictionary<string, string> headers,
            int timeOutInSeconds,
            string basicAuthUserName,
            string basicAuthPassword,
            Method method,
            DataFormat dataFormat,
            bool retryOnFailure = false,
            Uri proxyAddress = null)
        {
            var client = RestSharpHelper.BuildRestClient(requestUrl, timeOutInSeconds, basicAuthPassword,
                basicAuthUserName, proxyAddress);

            var requestConfiguration = new RestRequestConfiguration
            {
                Method = method,
                Headers = headers,
                DataFormat = dataFormat,
                Serializer = new CamelCaseSerializer()
            };

            var request = RestSharpHelper.BuildRequest(payload, requestConfiguration);

            var responsone = RestSharpHelper.PerformRestRequest(client, request, retryOnFailure);

            return responsone;
        }

        #endregion
    }
}
