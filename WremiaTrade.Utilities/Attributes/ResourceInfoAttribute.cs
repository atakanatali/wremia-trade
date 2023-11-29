namespace Papara.Utilities.Attributes
{
    using System;

    /// <summary>
    /// Attribute to decorate Resource Info Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ResourceInfoAttribute : Attribute
    {
        /// <summary>
        /// Key of Resource Info
        /// </summary>
        public string Key { get; set; }
    }
}
