namespace WremiaTrade.Services.Abstraction
{
    public class QueryRequestModel
    {
        /// <summary>
        /// How many records do you want to pass
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// How many records do you want to take
        /// </summary>
        public int Take { get; set; }
    }

    public class QueryResponseModel
    {
        /// <summary>
        /// How many record exist
        /// </summary>
        public int TotalItemCount { get; set; }
    }
}