using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Attachment
{
    public class OFAttachmentSimpleFilenameMatchPhrase : OFBaseMatchPhrase
    {
        public object filename { get; set; }

        public OFAttachmentSimpleFilenameMatchPhrase()
        {
            
        }
    }
}