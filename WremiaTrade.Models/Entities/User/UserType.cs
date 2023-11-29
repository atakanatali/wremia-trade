namespace WremiaTrade.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Kullanıcı tipi
    /// </summary>
    public enum UserType
    {
        [Display(Name = "Sahip")]
        Unapproved = 0,

        [Display(Name = "Paydaş")]
        Approved = 1
    }
}