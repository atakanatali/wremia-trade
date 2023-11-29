using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Papara.Services.Abstraction;

namespace Papara.Common.ApiHelpers
{
    /// <summary>
    /// Parametrenin null olup olmadığını kontrol eden attribute
    /// ModelState.IsValid kontrolü parametre null olduğunda true döndüğü için kullanıyoruz.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CheckModelForNullAttribute : ActionFilterAttribute
    {
        private readonly Func<Dictionary<string, object>, bool> _validate;

        /// <summary>
        /// 
        /// </summary>
        public CheckModelForNullAttribute()
            : this(arguments => arguments.ContainsValue(null))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkCondition"></param>
        public CheckModelForNullAttribute(Func<Dictionary<string, object>, bool> checkCondition)
        {
            _validate = checkCondition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.ActionArguments.Keys.Any())
            {
                var response = new ServiceResult(ServiceError.ModelStateError("Input cannot be null"));

                actionContext.Result = new OkObjectResult(response);
            }
        }
    }
}
