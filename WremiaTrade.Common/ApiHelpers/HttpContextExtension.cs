namespace WremiaTrade.Common.ApiHelpers
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;

    using Newtonsoft.Json;

    // using Papara.Logging.ElasticSearch;
    // using Papara.Services;
    // using Papara.Utilities;
    public static class HttpContextExtension
    {
        /// <summary>
        /// Cleansup the JSON body for when we're logging the requests.
        /// Cleansup the sensitive information like card numbers in the request content.
        /// </summary>
        public static string GetCleanRequestBody(string body, string[] ignoredLogFields, string[] maskedLogFields)
        {
            body = body
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("\t", "");

            dynamic modelObject = null!;

            try
            {
                modelObject = JsonConvert.DeserializeObject(body)!;
            }
            catch (JsonReaderException)
            {
                // When the request body comes as query string it is returned to json format so that it can be deserialized.
                var collection = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(body);

                body = JsonConvert.SerializeObject(collection.Keys.ToDictionary(y => y, y => collection[y]));

                modelObject = JsonConvert.DeserializeObject(body)!;
            }

            if (ignoredLogFields != null! && ignoredLogFields.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                foreach (var currentField in ignoredLogFields)
                {
                    if (string.IsNullOrWhiteSpace(currentField) || modelObject[currentField] == null)
                        continue;

                    modelObject[currentField] = null;
                }
            }

            if (maskedLogFields != null! && maskedLogFields.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                foreach (var currentField in maskedLogFields)
                {
                    if (string.IsNullOrWhiteSpace(currentField) || modelObject[currentField] == null)
                        continue;

                    modelObject[currentField] = "***";
                }
            }

            if (modelObject.password != null)
            {
                modelObject.password = "***";
            }

            if (modelObject.Password != null)
            {
                modelObject.Password = "***";
            }

            if (modelObject.expirationDate != null)
            {
                modelObject.expirationDate = "***";
            }

            if (modelObject.ExpirationDate != null)
            {
                modelObject.ExpirationDate = "***";
            }

            if (modelObject.PhoneNumber != null)
            {
                modelObject.PhoneNumber = "***";
            }

            body = JsonConvert.SerializeObject(modelObject);

            return body;
        }

        /// <summary>
        /// Gets the IP address from context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetIpAddress(this HttpContext context)
        {
            string? ipAddress = context.Connection.RemoteIpAddress.ToString();

            if (context.Request.Headers.TryGetValue("CF-Connecting-IP", out var ipAddresses))
            {
                var ips = ipAddresses.ToList();

                if (ips.Count > 0)
                {
                    ipAddress = ips[0];
                }
            }

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                //ILogService logService = context.RequestServices.GetService(typeof(ILogService)) as ILogService;

                //logService.Error("Could not get client ip address");
            }

            return ipAddress!;

        }

        /// <summary>
        /// Prepares log information for logging
        /// </summary>
        public static dynamic GetLogInfo(this HttpRequest httpRequest, string userId)
        {
            var headers = httpRequest.Headers.Where(x => !x.Key.Equals("Authorization") && !x.Key.Equals("Cookie")).ToArray();

            return new
            {
                RequestPath = httpRequest.GetDisplayUrl(),
                QueryString = httpRequest.Query,
                RequestMethod = httpRequest.Method.ToString(),
                RequestHeaders = JsonConvert.SerializeObject(headers),
                UserId = userId
            };
        }
        
        /// <summary>
        /// Gets body as raw(Important:this method will reset body position twice due async operation)
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="encoding">Default is null, pass encoding if need</param>
        /// <returns>Raw body</returns>
        public static async Task<string> GetRawBodyAsync(HttpRequest request, Encoding? encoding = null!)
        {
            if (!request.Body.CanSeek)
            {
                // We only do this if the stream isn't *already* seekable,
                // as EnableBuffering will create a new stream instance
                // each time it's called
                request.EnableBuffering();
            }

            request.Body.Position = 0;

            var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);

            var body = await reader.ReadToEndAsync().ConfigureAwait(false);

            request.Body.Position = 0;

            return body;
        }
    }
}