using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact.MatchPhrase
{
    public class OFContactFirstNameMatchPhrase : OFBaseMatchPhrase
    {
        public string firstname { get; set; }

        public OFContactFirstNameMatchPhrase()
        {

        }
    }
}