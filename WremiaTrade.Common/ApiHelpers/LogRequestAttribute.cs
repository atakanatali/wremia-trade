using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Papara.Logging.ElasticSearch;
using Papara.Logging.ElasticSearch.Model;
using Papara.Services.Interface;
using Papara.Utilities;
using Papara.Utilities.Attributes;
using Papara.Utilities.Extensions;

namespace Papara.Common.ApiHelpers
{
    /// <summary>
    /// Logs the request and response content on the decorated action 
    /// </summary>
    public class LogRequestAttribute : ActionFilterAttribute
    {
        private static ILogService _logService;

        private static IConfigurationService _configurationService;

        public bool TraceUser { get; set; } = false;

        public bool TraceEmail { get; set; } = false;

        public bool TracePhoneNumber { get; set; } = false;

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            try
            {
                if (_logService == null)
                {
                    _logService = (ILogService)actionContext.HttpContext.RequestServices.GetService(typeof(ILogService));
                }

                if (!ShouldTrace(actionContext))
                {
                    base.OnActionExecuting(actionContext);
                    return;
                }

                var request = actionContext.HttpContext.Request;

                var absoluteUri = string.Concat(
                       request.Scheme,
                       "://",
                       request.Host.ToUriComponent(),
                       request.PathBase.ToUriComponent(),
                       request.Path.ToUriComponent(),
                       request.QueryString.ToUriComponent());

                var headers = request.Headers.Where(x => !x.Key.Equals("Authorization") && !x.Key.Equals("Cookie")).ToArray();

                var jsonHeaders = JsonConvert.SerializeObject(headers);

                var ip = request.HttpContext.GetIpAddress();

                var logData = HttpContextExtension.GetRawBodyAsync(request).GetAwaiter().GetResult();

                var requestBody = JsonConvert.SerializeObject(logData);

                var requestModelType = actionContext.ActionArguments.Values.FirstOrDefault()?.GetType();

                try
                {
                    logData = JsonConvert.SerializeObject(MaskAndIgnoreFields(logData, requestModelType));
                }
                // RequestData can be wrongly formatted so we should not prevent logging for the sake of masking
                catch
                {
                    // Ignore exception
                }

                _logService.RequestResponseLog(new RequestResponseLogMessage
                {
                    Uri = absoluteUri,
                    IP = ip,
                    Request = requestBody,
                    LogDate = DateTime.Now,
                    Response = requestBody
                });
            }
            catch
            {
                //Log alinamadi
            }

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(ActionExecutedContext actionContext)
        {
            try
            {
                // Önemli: Her seferinde servisi cagirmazsak, icerisinde kullanilan datacontext'ler ilk kullanimdan sonra dispose oluyor bundan dolayida hata aliniyor.
                // Burada if kontrolü null ise servisi getir dememeliyiz.
                if (_logService == null)
                {
                    _logService = (ILogService)actionContext.HttpContext.RequestServices.GetService(typeof(ILogService));
                }

                if (!ShouldTrace(actionContext))
                {
                    base.OnActionExecuted(actionContext);
                    return;
                }

                var request = actionContext.HttpContext.Request;

                var absoluteUri = string.Concat(
                     request.Scheme,
                     "://",
                     request.Host.ToUriComponent(),
                     request.PathBase.ToUriComponent(),
                     request.Path.ToUriComponent(),
                     request.QueryString.ToUriComponent());

                var headers = request.Headers.Where(x => !x.Key.Equals("Authorization") && !x.Key.Equals("Cookie")).ToArray();

                var jsonHeaders = JsonConvert.SerializeObject(headers);
                var ip = request.HttpContext.GetIpAddress();
                var logData = HttpContextExtension.GetRawBodyAsync(request).GetAwaiter().GetResult();
                var contentResult = (actionContext.Result as OkObjectResult);
                _logService.Info($"actionContext.Result : {actionContext.Result}");
                if (contentResult == null)
                {
                    _logService.Info($"Content Result is null, Url : {absoluteUri}");
                    return;
                }

                var requestBody = JsonConvert.SerializeObject(logData);

                var response = JsonConvert.SerializeObject(contentResult.Value, contentResult.DeclaredType, null);

                var requestLogData = string.Empty;

                try
                {
                    requestLogData = JsonConvert.SerializeObject(MaskAndIgnoreFields(logData, contentResult.Value.GetType()));
                }
                // RequestData can be wrongly formatted so we should not prevent logging for the sake of masking
                catch
                {
                    // Ignore exception
                }

                _logService.RequestResponseLog(new RequestResponseLogMessage
                {
                    Uri = absoluteUri,
                    IP = ip,
                    Request = requestBody,
                    LogDate = DateTime.Now,
                    Response = response
                });
            }
            catch
            {
                // Log alinamadi
            }
            finally 
            {
                base.OnActionExecuted(actionContext);
            }
            
        }

        // Checks if the request should be traced based on user email, phone number or userid
        public bool ShouldTrace(FilterContext actionContext)
        {
            if (_configurationService == null)
            {
                _configurationService = (IConfigurationService)actionContext.HttpContext.RequestServices.GetService(typeof(IConfigurationService));
            }

            if (TraceEmail)
            {
                _configurationService.ReadConfiguration("TracedEmail", out string TracedEmails, false);

                var requestBody = HttpContextExtension.GetRawBodyAsync(actionContext.HttpContext.Request).GetAwaiter().GetResult();

                var cleanRequestBody = HttpContextExtension.GetCleanRequestBody(requestBody, null, null);

                var jsonBody = JsonConvert.DeserializeObject<JObject>(cleanRequestBody);

                var email = GetKeyFromBody(jsonBody, "Email", "email", "eMail");

                if (string.IsNullOrWhiteSpace(TracedEmails) || email == null)
                {
                    return false;
                }

                if (!TracedEmails.Contains(email))
                {
                    return false;
                }
            }

            if (TracePhoneNumber)
            {
                _configurationService.ReadConfiguration("TracedPhoneNumber", out string TracedPhoneNumbers, false);

                var cleanRequest = HttpContextExtension.GetRawBodyAsync(actionContext.HttpContext.Request).GetAwaiter().GetResult();

                var cleanRequestBody = HttpContextExtension.GetCleanRequestBody(cleanRequest, null, null);

                var requestBody = JsonConvert.DeserializeObject<JObject>(cleanRequest);

                var phoneNumber = GetKeyFromBody(requestBody, "PhoneNumber", "phoneNumber", "phonenumber");

                if (string.IsNullOrWhiteSpace(TracedPhoneNumbers) || phoneNumber == null)
                {
                    return false;
                }

                if (!TracedPhoneNumbers.Contains(phoneNumber))
                {
                    return false;
                }
            }

            if (TraceUser)
            {
                _configurationService.ReadConfiguration("TracedUserId", out string TracedUserIds, false);

                if (!actionContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    return false;
                }

                string userId = actionContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(TracedUserIds))
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(TracedUserIds) && !TracedUserIds.Contains(userId))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets key from body. Tries each keyword.
        /// </summary>
        public static string GetKeyFromBody(JObject body, params string[] keywords)
        {
            foreach (var key in keywords)
            {
                if (body[key] != null)
                {
                    return body[key].ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Checks ignored and masked fields of given object data
        /// </summary>
        public static object MaskAndIgnoreFields(string json, Type type)
        {
            if (type == null)
            {
                return null;
            }

            var clonedModel = JsonConvert.DeserializeObject(json, type);

            PropertyInfo[] properties = clonedModel?.GetPropertiesOfAnObject();

            var currentModel = JObject.Parse(json);

            var ignoredLogFields = properties.GetAttributeInfo<IgnoredLogAttribute>();

            if (ignoredLogFields != null && ignoredLogFields.Any(x => !string.IsNullOrWhiteSpace(x.Key)))
            {
                foreach (var currentField in ignoredLogFields)
                {
                    var currentLogDataProp = currentModel.FindTokens(currentField.Key);

                    if (currentLogDataProp == null && currentLogDataProp.Count == 0)
                    {
                        continue;
                    }

                    foreach (var prop in currentLogDataProp.OfType<JValue>())
                    {
                        prop.Value = null;
                    }
                }
            }

            var maskedLogFields = properties.GetAttributeInfo<MaskedLogAttribute>();

            if (maskedLogFields != null && maskedLogFields.Any(x => !string.IsNullOrWhiteSpace(x.Key)))
            {
                foreach (var currentField in maskedLogFields)
                {
                    var currentLogDataProp = currentModel.FindTokens(currentField.Key);

                    if (currentLogDataProp == null && currentLogDataProp.Count == 0)
                    {
                        continue;
                    }

                    foreach (var prop in currentLogDataProp.OfType<JValue>())
                    {
                        if (currentField.Value == LoggingStyle.Default)
                        {
                            prop.Value = "***";
                        }

                        if (currentField.Value == LoggingStyle.TurkishNationalId)
                        {
                            prop.Value = MaskingHelper.GenerateMaskedTCKN(prop.Value?.ToString());
                        }

                        if (currentField.Value == LoggingStyle.CardNo)
                        {
                            prop.Value = MaskingHelper.GenerateMaskedPan(prop.Value?.ToString());
                        }

                        if (currentField.Value == LoggingStyle.PhoneNumber)
                        {
                            prop.Value = MaskingHelper.GenerateMaskedPhoneNumber(prop.Value?.ToString());
                        }
                    }
                }
            }

            return currentModel;
        }
    }
}