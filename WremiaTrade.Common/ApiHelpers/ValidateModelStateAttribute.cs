namespace Papara.Common.ApiHelpers
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Papara.Logging.ElasticSearch;
    using Papara.Services.Abstraction;

    /// <summary>
    /// Attribute to prevent modelstate controls in each method
    /// </summary>
    public sealed class ValidateModelStateAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Model valid değil ise 400 durum kodu ile model hatalarını döner
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var _logService = actionContext.HttpContext.RequestServices.GetService(typeof(ILogService)) as ILogService;
            _logService.Info("ValidateModelStateAttribute-OnActionExecuting-logService created");

            if (!actionContext.ModelState.IsValid)
            {

                var error = actionContext.ModelState.Values.SelectMany(m => m.Errors).First().ErrorMessage;

                /*
                 * Some validation errors do not provide an ErrorMessage, in that case, we return the exception message.
                 */
                if (string.IsNullOrWhiteSpace(error))
                {
                    error = actionContext.ModelState.Values.SelectMany(m => m.Errors).First().Exception.Message;
                }

                var response = new ServiceResult(ServiceError.ModelStateError(error));

                actionContext.Result = new OkObjectResult(response);
            }
        }
    }
}
