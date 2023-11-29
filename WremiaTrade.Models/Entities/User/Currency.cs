namespace WremiaTrade.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Currency
    /// </summary>
    public enum Currency
    {
        [Display(Name = "Türk Lirası")]
        TRY = 0,

        [Display(Name = "Amerikan Doları")]
        USD = 1,

        [Display(Name = "Euro")]
        EUR = 2,

        [Display(Name = "Bitcoin")]
        BTC = 3,

        [Display(Name = "Sterlin")]
        GBP = 4
    }
}