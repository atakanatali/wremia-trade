namespace Papara.Utilities.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Checks that at least one field of the object is filled
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AtLeastOneFilledPropertyAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var typeInfo = value.GetType();

            var propertyInfo = typeInfo.GetProperties();

            foreach (var property in propertyInfo)
            {
               var attributes = property.GetCustomAttributes(typeof(AtLeastOneAttribute), true).FirstOrDefault();

                if (attributes == null)
                {
                    continue;
                }

                if (null != property.GetValue(value, null))
                {
                    if (property.PropertyType.Name == "String" && Convert.ToString(property.GetValue(value, null)) == "+9")
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }
        
    }
}
