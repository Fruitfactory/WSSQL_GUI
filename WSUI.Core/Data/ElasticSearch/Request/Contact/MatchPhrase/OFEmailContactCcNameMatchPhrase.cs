using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact.MatchPhrase
{
    public class OFEmailContactCcNameMatchPhrase : OFBaseMatchPhrase
    {

        [JsonProperty("cc.name")]
        public string ccName { get; set; }
        
    }
}