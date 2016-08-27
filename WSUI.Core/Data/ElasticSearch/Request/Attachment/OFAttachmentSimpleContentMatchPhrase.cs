using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Attachment
{
    public class OFAttachmentSimpleContentMatchPhrase : OFBaseMatchPhrase
    {
        public object analyzedcontent { get; set; }

        public OFAttachmentSimpleContentMatchPhrase()
        {
            
        }
    }
}