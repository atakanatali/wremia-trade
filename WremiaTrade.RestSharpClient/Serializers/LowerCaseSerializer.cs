namespace WremiaTrade.RestSharpClient.Serializers
{
    using Newtonsoft.Json;

    using WremiaTrade.RestSharpClient.JsonContractResolvers;

    using RestSharp.Serializers;
    
    /// <summary>
    /// Camel Case Serilaizer for Restsharp
    /// </summary>
    public class LowerCaseSerializer : ISerializer
    {
        /// <summary>
        /// Default serializer
        /// </summary>
        public LowerCaseSerializer()
        {
            ContentType = "application/json";
        }

        /// <summary>
        /// Serialize the object as JSON
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON as String</returns>
        public string Serialize(object obj)
        {
            var lowerCaseSetting = new JsonSerializerSettings
            {
                ContractResolver = new LowercaseContractResolver()
            };

            string lowerCaseJson = JsonConvert.SerializeObject(obj, lowerCaseSetting);

            return lowerCaseJson;
        }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string RootElement { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Content type for serialized content
        /// </summary>
        public string ContentType { get; set; }
    }
}
