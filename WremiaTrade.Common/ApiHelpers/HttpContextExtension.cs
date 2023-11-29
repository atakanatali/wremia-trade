using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

using Newtonsoft.Json;

using Papara.Logging.ElasticSearch;
using Papara.Services;
using Papara.Utilities;

namespace Papara.Common.ApiHelpers
{
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

            //PapararCard binlerimizi buluyoruz.
            var matchCollection = Regex.Matches(body, $@"(?={PaparaCardUtilities.PaparaCardBins})\w{{16}}");

            foreach (Match match in matchCollection)
            {
                string maskedPan = $"{match.Value.Substring(0, 4)} {match.Value.Substring(4, 2)}** **** {match.Value.Substring(12, 4)}";

                body = body.Replace(match.Value, maskedPan);
            }

            dynamic modelObject = null;

            try
            {
                modelObject = JsonConvert.DeserializeObject(body);
            }
            catch (JsonReaderException)
            {
                // When the request body comes as query string it is returned to json format so that it can be deserialized.
                var collection = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(body);

                body = JsonConvert.SerializeObject(collection.Keys.ToDictionary(y => y, y => collection[y]));

                modelObject = JsonConvert.DeserializeObject(body);
            }

            if (ignoredLogFields != null && ignoredLogFields.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                foreach (var currentField in ignoredLogFields)
                {
                    if (string.IsNullOrWhiteSpace(currentField) || modelObject[currentField] == null)
                        continue;

                    modelObject[currentField] = null;
                }
            }

            if (maskedLogFields != null && maskedLogFields.Any(x => !string.IsNullOrWhiteSpace(x)))
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

            if (modelObject.pin != null)
            {
                modelObject.pin = "***";
            }

            if (modelObject.Pin != null)
            {
                modelObject.Pin = "***";
            }

            if (modelObject.oldPin != null)
            {
                modelObject.oldPin = "***";
            }

            if (modelObject.OldPin != null)
            {
                modelObject.OldPin = "***";
            }

            if (modelObject.newPin != null)
            {
                modelObject.newPin = "***";
            }

            if (modelObject.NewPin != null)
            {
                modelObject.NewPin = "***";
            }

            if (modelObject.Cvv2 != null)
            {
                modelObject.Cvv2 = "***";
            }

            if (modelObject.cvv2 != null)
            {
                modelObject.cvv2 = "***";
            }

            if (modelObject.Cvv != null)
            {
                modelObject.Cvv = "***";
            }

            if (modelObject.cvv != null)
            {
                modelObject.cvv = "***";
            }

            if (modelObject.expirationDate != null)
            {
                modelObject.expirationDate = "***";
            }

            if (modelObject.ExpirationDate != null)
            {
                modelObject.ExpirationDate = "***";
            }

            if (modelObject.TfaCode != null)
            {
                modelObject.TfaCode = "***";
            }

            if (modelObject.tfaCode != null)
            {
                modelObject.tfaCode = "***";
            }

            if (modelObject.humanVerificationToken != null)
            {
                //modelObject.humanVerificationToken = "***";
            }

            if (modelObject.HumanVerificationToken != null)
            {
                //modelObject.HumanVerificationToken = "***";
            }

            if (modelObject.TurkishNationalId != null)
            {
                modelObject.TurkishNationalId = MaskingHelper.GenerateMaskedTCKN((string)modelObject.TurkishNationalId);
            }

            if (modelObject.turkishNationalId != null)
            {
                modelObject.turkishNationalId = MaskingHelper.GenerateMaskedTCKN((string)modelObject.turkishNationalId);
            }

            if (modelObject.turkishnationalid != null)
            {
                modelObject.turkishnationalid = MaskingHelper.GenerateMaskedTCKN((string)modelObject.turkishnationalid);
            }

            if (modelObject.SerialNumber != null)
            {
                modelObject.SerialNumber = "***";
            }

            if (modelObject.IdentityNo != null)
            {
                modelObject.IdentityNo = "***";
            }

            if (modelObject.IdentitySerial != null)
            {
                modelObject.IdentitySerial = "***";
            }

            if (modelObject.phoneNumber != null)
            {
                modelObject.phoneNumber = "***";
            }

            if (modelObject.PhoneNumber != null)
            {
                modelObject.PhoneNumber = "***";
            }

            if (modelObject.cardno != null)
            {
                var cardNo = modelObject.cardno.ToString();
                var maskedPan = $"{cardNo.Substring(0, 4)} {cardNo.Substring(4, 2)}** **** {cardNo.Substring(12, 4)}";

                modelObject.cardno = maskedPan;
            }

            if (modelObject.CardNo != null)
            {
                var cardNo = modelObject.CardNo.ToString();
                var maskedPan = $"{cardNo.Substring(0, 4)} {cardNo.Substring(4, 2)}** **** {cardNo.Substring(12, 4)}";

                modelObject.CardNo = maskedPan;
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
            string ipAddress = context.Connection.RemoteIpAddress.ToString();

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
                ILogService logService = context.RequestServices.GetService(typeof(ILogService)) as ILogService;

                logService.Error("Could not get client ip address");
            }

            return ipAddress;

        }

        /// <summary>
        /// Prepares log information for logging
        /// </summary>
        public static dynamic GetLogInfo(this HttpRequest httpRequest, string userId)
        {
            string requestBody = string.Empty;
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
        public static async Task<string> GetRawBodyAsync(HttpRequest request, Encoding encoding = null)
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

        /// <summary>
        /// Masks the credit card information
        /// </summary>
        private static void MaskCreditCard(dynamic data)
        {
            if (data.cardnumber != null)
            {
                data.cardnumber = "***";
            }

            if (data.cvv != null)
            {
                data.cvv = "***";
            }

            if (data.expirymonth != null)
            {
                data.expirymonth = "***";
            }

            if (data.expiryyear != null)
            {
                data.expiryyear = "***";
            }
        }
    }
}