namespace Papara.Utilities.Attributes
{
    using System;

    /// <summary>
    /// Mask attribute for logging <see cref="LoggingIgnoreAttribute"/>
    /// </summary>

    [AttributeUsage(AttributeTargets.Property)]
    public class MaskedLogAttribute : Attribute
    {
        public LoggingStyle LoggingStyle { get; set; }
    }
}