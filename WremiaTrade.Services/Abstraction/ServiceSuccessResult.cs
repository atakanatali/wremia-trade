namespace WremiaTrade.Services.Abstraction
{
    using Papara.Services.Resources;

    /// <summary>
    /// Represents a class for succeeded service results
    /// </summary>
    public class ServiceSuccessResult : BaseServiceResult
    {
        public ServiceSuccessResult(string message, int code) : base(message, code)
        {
        }

        public static ServiceSuccessResult DefaultServiceSuccessResult => new ServiceSuccessResult(SuccessMessageResource.DefaultSucessMessage, 101);

        public static ServiceSuccessResult PaparaCardActivated => new ServiceSuccessResult(SuccessMessageResource.PaparaCardActivated, 102);

        public static ServiceSuccessResult PaparaCardActivatedAndFeeRefunded => new ServiceSuccessResult(SuccessMessageResource.PaparaCardActivatedAndFeeRefunded, 103);
    }
}
