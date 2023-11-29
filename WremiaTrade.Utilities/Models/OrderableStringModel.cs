namespace WremiaTrade.Utilities.Models
{
    /// <summary>
    /// Orderable string model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrderableStringModel<T> where T : struct
    {
        /// <summary>
        /// Identity number
        /// </summary>
        public T Id { get; set; }

        /// <summary>
        /// Sorting order number
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }
    }
}
