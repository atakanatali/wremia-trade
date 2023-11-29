using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Papara.Services.Abstraction;
using Papara.Services.Resources;

namespace Papara.Common.ApiHelpers
{
    /// <summary>
    /// Attribute to prevent modelstate controls in each method
    /// </summary>
    public sealed class ValidateModelStateI18NAttribute : ActionFilterAttribute
    {

        private IResourceReadService _resourceReadService;
        /// <summary>
        /// If the model is not valid, it returns 400 status code and model errors, which are coming from resource read service
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (_resourceReadService == null)
            {
                _resourceReadService = actionContext.HttpContext.RequestServices.GetService(typeof(IResourceReadService)) as IResourceReadService;
            }

            if (!actionContext.ModelState.IsValid)
            {
                var error = actionContext.ModelState.Values
                    .SelectMany(m => m.Errors).FirstOrDefault(t => !string.IsNullOrEmpty(t.ErrorMessage)) ??
                            actionContext.ModelState.Values.SelectMany(m => m.Errors).First();

                var errorMessage = string.IsNullOrWhiteSpace(error.ErrorMessage) ? error.Exception.Message : error.ErrorMessage;

                var resourceValue = _resourceReadService.GetResource<string>(errorMessage);

                var validationError = string.IsNullOrEmpty(resourceValue) ? errorMessage : resourceValue;

                var response = new ServiceResult(ServiceError.ModelStateError(validationError));

                actionContext.Result = new OkObjectResult(response);
            }
        }
    }
}
