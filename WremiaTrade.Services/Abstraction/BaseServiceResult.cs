namespace WremiaTrade.Services.Abstraction
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a base class for Service result objects
    /// </summary>
    public abstract class BaseServiceResult
    {
        [Required]
        public string Message { get; set; }

        [Required]
        public int Code { get; set; }

        protected BaseServiceResult(string message, int code)
        {
            Message = message;
            Code = code;
        }
    }
}
