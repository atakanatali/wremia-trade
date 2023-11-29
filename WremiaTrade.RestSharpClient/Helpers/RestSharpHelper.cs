namespace WremiaTrade.RestSharpClient.Helpers
{
    using System.Net;

    using RestSharp;
    using RestSharp.Authenticators;

    using WremiaTrade.RestSharpClient.Configuration;
    using WremiaTrade.RestSharpClient.Exceptions;
    
    /// <summary>
    /// Helper methods for restsharp client
    /// </summary>
    public static class RestSharpHelper
    {
        /// <summary>
        /// Performs rest request
        /// </summary>
        public static IRestResponse PerformRestRequest(IRestClient client, IRestRequest request, bool retryOnFailure = false)
        {
#if DEBUG
            //The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.
            //for development purposes only
            //skip ssl verification
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
#endif
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            IRestResponse response = client.Execute(request);

            if (response.ResponseStatus != ResponseStatus.Completed && retryOnFailure)
            {
                response = client.Execute(request);
            }

            if (!response.IsSuccessful && response.ErrorException != null)
            {
                throw new RestSharpException(client.BaseUrl.AbsoluteUri, response.ErrorException);
            }

            return response;
        }

        /// <summary>
        /// It prepares rest client specifications
        /// </summary>
        public static IRestClient BuildRestClient(string requestUrl, int timeOutInSeconds, string basicAuthPassword, string basicAuthUserName, Uri proxyAddress)
        {
            var client = new RestClient(requestUrl)
            {
                Timeout = timeOutInSeconds * 1000
            };

            if (!string.IsNullOrWhiteSpace(basicAuthUserName) && !string.IsNullOrWhiteSpace(basicAuthPassword))
            {
                client.Authenticator = new HttpBasicAuthenticator(basicAuthUserName, basicAuthPassword);
            }

            if (proxyAddress != default)
            {
                client.Proxy = new WebProxy(proxyAddress, true, null, null);
            }

            return client;
        }

        /// <summary>
        /// It prepares rest request specifications
        /// </summary>
        public static IRestRequest BuildRequest(dynamic payload, RestRequestConfiguration configuration)
        {
            var request = new RestRequest(configuration.Method)
            {
                RequestFormat = configuration.DataFormat
            };

            if (configuration.DataFormat == DataFormat.Json && configuration.Serializer != null)
            {
                request.JsonSerializer = configuration.Serializer;
            }

            if (configuration.Headers?.Count > 0)
            {
                foreach (KeyValuePair<string, string> header in configuration.Headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            if (payload is null) return request;

            if (configuration.Headers != null && configuration.Headers.ContainsKey("Content-Type"))
            {
                string contentType = configuration.Headers["Content-Type"];

                switch (contentType)
                {
                    case "application/x-www-form-urlencoded":
                        request.AddObject(payload);
                        break;
                    case "application/json":
                        request.AddJsonBody(payload);
                        break;
                    case "application/xml":
                        request.AddBody(payload);
                        break;
                    default:
                        request.AddBody(payload);
                        break;
                }
            }
            else
            {
                request.AddBody(payload);
            }

            return request;
        }

        /// <summary>
        /// It supports stream operations(ex. DownloadStream)
        /// </summary>
        /// <param name="client">Client itself</param>
        /// <param name="request">Rest request</param>
        /// <returns></returns>
        public static byte[] DownloadData(IRestClient client, IRestRequest request)
        {
#if DEBUG
            //The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.
            //for development purposes only
            //skip ssl verification
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
#endif
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            byte[] response = client.DownloadData(request);


            return response;
        }
    }
}
