namespace WremiaTrade.Common.ApiHelpers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using Newtonsoft.Json;

    // using Papara.Logging.ElasticSearch;
    // using Papara.Logging.ElasticSearch.Model;

    /// <summary>
    /// Logs the request and response content on the decorated action 
    /// </summary>
    public class LogRequestAttribute : ActionFilterAttribute
    {
        // private static ILogService _logService;
        public bool TraceUser { get; set; } = false;

        public bool TraceEmail { get; set; } = false;

        public bool TracePhoneNumber { get; set; } = false;

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            try
            {
                // if (_logService == null)
                // {
                //     _logService = (ILogService)actionContext.HttpContext.RequestServices.GetService(typeof(ILogService));
                // }

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
                
                // _logService.RequestResponseLog(new RequestResponseLogMessage
                // {
                //     Uri = absoluteUri,
                //     IP = ip,
                //     Request = requestBody,
                //     LogDate = DateTime.Now,
                //     Response = requestBody
                // });
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
                // if (_logService == null)
                // {
                //     _logService = (ILogService)actionContext.HttpContext.RequestServices.GetService(typeof(ILogService));
                // }
                
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
                
                // _logService.Info($"actionContext.Result : {actionContext.Result}");
                // if (contentResult == null)
                // {
                //     _logService.Info($"Content Result is null, Url : {absoluteUri}");
                //     return;
                // }

                var requestBody = JsonConvert.SerializeObject(logData);

                var response = JsonConvert.SerializeObject(contentResult.Value, contentResult.DeclaredType, null);

                // _logService.RequestResponseLog(new RequestResponseLogMessage
                // {
                //     Uri = absoluteUri,
                //     IP = ip,
                //     Request = requestBody,
                //     LogDate = DateTime.Now,
                //     Response = response
                // });
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
    }
}