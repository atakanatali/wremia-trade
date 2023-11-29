namespace Papara.Services.Abstraction
{
    using Newtonsoft.Json;

    public class PagingRequest
    {
        public PagingRequest(int pageIndex, int pageItemCount)
        {
            PageIndex = pageIndex;
            PageItemCount = pageItemCount;
        }

        public int PageIndex { get; }

        public int PageItemCount { get; }

        [JsonIgnore]
        public int PageSkip => PageIndex * PageItemCount - PageItemCount;
    }
}