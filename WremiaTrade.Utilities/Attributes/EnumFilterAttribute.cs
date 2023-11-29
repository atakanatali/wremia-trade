namespace WremiaTrade.Utilities.Attributes
{
    using System;

    /// <summary>
    /// Attribute that defines attached Auditlog is related with KYC
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EnumFilterAttribute : Attribute
    {
        public string FilterName { get; set; }

        public EnumFilterAttribute(string filterName)
        {
            FilterName = filterName;
        }
    }
}