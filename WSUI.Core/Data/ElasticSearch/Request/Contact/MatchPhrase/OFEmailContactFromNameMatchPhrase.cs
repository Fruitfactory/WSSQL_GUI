using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact.MatchPhrase
{
    public class OFEmailContactFromNameMatchPhrase : OFBaseMatchPhrase
    {
        [JsonProperty("fromname")]
        public string fromname { get; set; }
    }
}