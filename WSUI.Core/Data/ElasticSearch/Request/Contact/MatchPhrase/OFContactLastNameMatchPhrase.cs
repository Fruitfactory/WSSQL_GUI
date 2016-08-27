using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact.MatchPhrase
{
    public class OFContactLastNameMatchPhrase : OFBaseMatchPhrase
    {
        public string lastname { get; set; }

        public OFContactLastNameMatchPhrase()
        {

        }
    }
}