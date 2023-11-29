namespace WremiaTrade.RestSharpClient.JsonContractResolvers
{
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Ensures that all keys are resolved lower case when serializing
    /// </summary>
    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}