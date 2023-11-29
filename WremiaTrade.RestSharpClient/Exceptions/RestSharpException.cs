namespace WremiaTrade.RestSharpClient.Exceptions
{
    public class RestSharpException : Exception
    {
        public RestSharpException(string requestUrl, Exception ex) : base(requestUrl, ex)
        {
        }
    }
}