using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact.MatchPhrase
{
    public class OFEmailContactToNameMatchPhrase : OFBaseMatchPhrase
    {
        [JsonProperty("to.name")]
        public string toName { get; set; }   
    }
}