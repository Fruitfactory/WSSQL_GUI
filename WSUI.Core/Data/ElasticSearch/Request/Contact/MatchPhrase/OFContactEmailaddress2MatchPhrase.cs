using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact.MatchPhrase
{
    public class OFContactEmailaddress2MatchPhrase : OFBaseMatchPhrase
    {
        public string emailaddress2 { get; set; }

        public OFContactEmailaddress2MatchPhrase()
        {
            
        }
    }
}