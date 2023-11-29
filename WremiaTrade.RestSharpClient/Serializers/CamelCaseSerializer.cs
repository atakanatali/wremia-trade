namespace WremiaTrade.RestSharpClient.Serializers
{
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;

    using RestSharp.Serializers;
    
    /// <summary>
    /// Camel Case Serilaizer for Restsharp
    /// </summary>
    public class CamelCaseSerializer : ISerializer
    {
        /// <summary>
        /// Default serializer
        /// </summary>
        public CamelCaseSerializer()
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
            var camelCaseSetting = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string camelCaseJson = JsonConvert.SerializeObject(obj, camelCaseSetting);

            return camelCaseJson;
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
