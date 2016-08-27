using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact.MatchPhrase
{
    public class OFEmailContactBccAddressMatchPhrase : OFBaseMatchPhrase
    {
        [JsonProperty("bcc.address")]
        public string bccAddress { get; set; }
    }
}