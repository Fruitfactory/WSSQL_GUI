using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Attachment
{
    public class OFAttachmentSimpleFilenameWildcard : OFBaseWildcard
    {
        public object filename { get; set; }

        public OFAttachmentSimpleFilenameWildcard()
        {
            
        }

        public override void SetValue(object value)
        {
            filename = string.Format("{0}*", value);
        }
    }
}