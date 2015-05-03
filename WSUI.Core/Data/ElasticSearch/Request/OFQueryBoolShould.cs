using Newtonsoft.Json;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFQueryBoolShould<T> where T: class
    {
        [JsonProperty("bool")]
        public OFBoolShould<T> _bool { get; set; }

          public OFQueryBoolShould()
        {
            _bool = new OFBoolShould<T>();
        }
    }
}