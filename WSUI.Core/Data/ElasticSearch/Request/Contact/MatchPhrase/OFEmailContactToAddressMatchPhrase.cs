using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact.MatchPhrase
{
    public class OFEmailContactToAddressMatchPhrase : OFBaseMatchPhrase
    {
        [JsonProperty("to.address")]
        public string toAddress { get; set; }
        
    }
}