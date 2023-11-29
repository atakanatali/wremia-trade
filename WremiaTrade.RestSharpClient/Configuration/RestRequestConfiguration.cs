namespace WremiaTrade.RestSharpClient.Configuration
{
    using RestSharp.Serializers;
    using RestSharp;
    
    /// <summary>
    /// Configure request
    /// </summary>
    public class RestRequestConfiguration
    {
        public ISerializer Serializer { get; set; }

        public Method Method { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public DataFormat DataFormat { get; set; }
    }
}