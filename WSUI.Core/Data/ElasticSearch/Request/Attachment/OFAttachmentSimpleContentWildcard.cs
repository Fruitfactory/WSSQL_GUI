using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Attachment
{
    public class OFAttachmentSimpleContentWildcard : OFBaseWildcard
    {
        public object analyzedcontent { get; set; }

        public OFAttachmentSimpleContentWildcard()
        {
            
        }

        public override void SetValue(object value)
        {
            analyzedcontent = string.Format("*{0}*", value);
        }
    }
}