using Newtonsoft.Json;
using OF.Core.Interfaces;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFQueryMatchPhrase<T> : IOFQueryMatchPhrase<T> where T : new()
    {
        [JsonProperty("match_phrase")]
        public T matchphrase { get; set; }

        public OFQueryMatchPhrase(T searchProperty)
        {
            matchphrase = searchProperty;
        }
    }
}