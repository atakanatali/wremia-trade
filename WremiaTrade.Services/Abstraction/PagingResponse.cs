namespace WremiaTrade.Services.Abstraction
{
    using System.Collections.Generic;

    /// <summary>
    /// Sayfalama işlemleri modeli
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagingResponse<T>
    {
        /// <summary>
        /// Toplam Sayfa sayısı
        /// </summary>
        public int TotalPageCount { get; set; }

        /// <summary>
        /// Toplam kayıt sayısı
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// Kayıtlar
        /// </summary>
        public IEnumerable<T> Items { get; set; }
    }
}
