namespace WremiaTrade.Utilities.Attributes
{
    using System;

    /// <summary>
    /// This attribute is used for return custom message for enum 
    /// </summary>
    public class CustomDisplayErrorAttribute : Attribute
    {
        public CustomDisplayErrorAttribute(string errorMessage, int errorCode = 0)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Error Message
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Error Code
        /// </summary>
        public int ErrorCode { get; private set; }
    }
}