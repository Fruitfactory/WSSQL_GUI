using Newtonsoft.Json.Serialization;

namespace OF.Core.JsonSettings
{
    public class OFLowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLowerInvariant();
        }
    }
}