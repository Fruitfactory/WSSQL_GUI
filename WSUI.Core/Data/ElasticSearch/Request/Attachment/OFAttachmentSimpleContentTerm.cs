using System.Collections.Generic;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Attachment
{
    public class OFAttachmentSimpleContentTerm : OFBaseTerm
    {
        public Dictionary<string, object> analyzedcontent { get; set; }

        public OFAttachmentSimpleContentTerm()
        {
            analyzedcontent = new Dictionary<string, object>();
        }

        public override void SetValue(object value)
        {
            analyzedcontent.Add("value",value);
        }
    }
}