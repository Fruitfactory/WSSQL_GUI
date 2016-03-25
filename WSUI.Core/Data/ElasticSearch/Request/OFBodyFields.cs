using Newtonsoft.Json;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFBodyFields : OFBody
    {
        [JsonProperty("_source")]
        public object fields { get; set; } 
    }
}