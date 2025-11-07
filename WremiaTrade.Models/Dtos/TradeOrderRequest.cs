namespace WremiaTrade.Models.Dtos
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using WremiaTrade.Models.Entities.Trade;

    public class TradeOrderRequest
    {
        [Required]
        [MaxLength(16)]
        public string Symbol { get; set; } = string.Empty;

        [Range(0.0000001, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        public TradeOrderSide Side { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal? LimitPrice { get; set; }

        public TimeSpan? ExecuteAfter { get; set; }

        [MaxLength(256)]
        public string? Notes { get; set; }
    }
}
