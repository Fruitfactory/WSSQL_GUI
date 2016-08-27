using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact.MatchPhrase
{
    public class OFEmailContactCcAddressMatchPhrase : OFBaseMatchPhrase
    {
        [JsonProperty("cc.address")]
        public string ccAddress { get; set; }
    }
}