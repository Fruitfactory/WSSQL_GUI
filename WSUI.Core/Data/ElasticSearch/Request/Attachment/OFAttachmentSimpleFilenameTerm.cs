using System.Collections.Generic;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Attachment
{
    public class OFAttachmentSimpleFilenameTerm : OFBaseTerm
    {
        public Dictionary<string,object> filename { get; set; }

        public OFAttachmentSimpleFilenameTerm()
        {
            filename = new Dictionary<string, object>();
        }

        public override void SetValue(object value)
        {
            filename.Add("value",value);
        }
    }


    public class OFFilenameTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "filename";
        }
    }
}