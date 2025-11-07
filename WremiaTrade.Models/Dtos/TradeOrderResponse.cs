namespace WremiaTrade.Models.Dtos
{
    public class TradeOrderResponse
    {
        public TradeOrderDto Order { get; set; } = new TradeOrderDto();

        public string Message { get; set; } = string.Empty;
    }
}
