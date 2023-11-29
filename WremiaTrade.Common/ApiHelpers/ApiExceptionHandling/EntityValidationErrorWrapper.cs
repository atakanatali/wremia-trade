using System.Collections.Generic;
using System.Data.Entity.Validation;

using Newtonsoft.Json;

namespace Papara.Common.ApiHelpers.ApiExceptionHandling
{
    /// <summary>
    /// Wraps the entity validation error (result) on DbEntityValidationException
    /// </summary>
    public class EntityValidationErrorWrapper
    {
        /// <summary>
        /// Name of the entity that gets validation exception
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Validation exception error details
        /// </summary>
        public IList<DbValidationError> ValidationErrors { get; set; }
    }

    /// <summary>
    /// Holds all entity validation errors on the exception
    /// </summary>
    public class EntityValidationErrors
    {
        /// <summary>
        /// Contains all entity/property level validation errors 
        /// </summary>
        public IList<EntityValidationErrorWrapper> Errors { get; set; }

        public EntityValidationErrors()
        {
            Errors = new List<EntityValidationErrorWrapper>();
        }

        /// <summary>
        /// Converts the validation errors to stringified json
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
