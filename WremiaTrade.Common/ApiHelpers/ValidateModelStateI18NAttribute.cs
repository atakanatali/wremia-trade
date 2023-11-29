namespace WremiaTrade.Common.ApiHelpers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using WremiaTrade.Services.Abstraction;

    /// <summary>
    /// Attribute to prevent modelstate controls in each method
    /// </summary>
    public sealed class ValidateModelStateI18NAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// If the model is not valid, it returns 400 status code and model errors, which are coming from resource read service
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (actionContext.ModelState.IsValid) return;
            var error = actionContext.ModelState.Values
                            .SelectMany(m => m.Errors).FirstOrDefault(t => !string.IsNullOrEmpty(t.ErrorMessage)) ??
                        actionContext.ModelState.Values.SelectMany(m => m.Errors).First();

            var errorMessage = string.IsNullOrWhiteSpace(error.ErrorMessage) ? error.Exception.Message : error.ErrorMessage;
            
            var response = new ServiceResult(ServiceError.ModelStateError(errorMessage));

            actionContext.Result = new OkObjectResult(response);
        }
    }
}
