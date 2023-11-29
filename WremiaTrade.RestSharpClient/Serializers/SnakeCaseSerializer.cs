using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

using RestSharp.Serializers;

namespace Papara.RestSharpClient.Serializers
{
    /// <summary>
    /// Snake Case Serilaizer for Restsharp
    /// </summary>
    public class SnakeCaseSerializer : ISerializer
    {
        /// <summary>
        /// Default serializer
        /// </summary>
        public SnakeCaseSerializer()
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
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            string snakeCaseJson = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            return snakeCaseJson;
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
