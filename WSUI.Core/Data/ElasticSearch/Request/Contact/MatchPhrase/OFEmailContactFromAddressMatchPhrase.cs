using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact.MatchPhrase
{
    public class OFEmailContactFromAddressMatchPhrase : OFBaseMatchPhrase
    {
        [JsonProperty("fromaddress")]
        public string fromaddress { get; set; }
    }
}