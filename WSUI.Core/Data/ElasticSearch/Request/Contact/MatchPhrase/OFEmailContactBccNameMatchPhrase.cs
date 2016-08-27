using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact.MatchPhrase
{
    public class OFEmailContactBccNameMatchPhrase : OFBaseMatchPhrase
    {
        [JsonProperty("bcc.name")]
        public string bccName { get; set; }
    }
}