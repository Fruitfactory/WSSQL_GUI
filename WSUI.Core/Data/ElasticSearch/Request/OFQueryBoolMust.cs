using Newtonsoft.Json;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFQueryBoolMust<T> where T : new()
    {
        [JsonProperty("bool")]
        public OFBoolMust<T> _bool { get; set; }

        public OFQueryBoolMust()
        {
            _bool = new OFBoolMust<T>();
        }
    }
}