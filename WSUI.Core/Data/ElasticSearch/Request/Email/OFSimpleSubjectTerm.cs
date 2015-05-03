using System.Collections.Generic;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Email
{
    public class OFSimpleSubjectTerm : OFBaseTerm
    {
        public Dictionary<string,object> subject { get; set; }

        public OFSimpleSubjectTerm()
        {
            subject = new Dictionary<string, object>();
        }

        public override void SetValue(object value)
        {
            subject.Add("value", value);
        }
    }
}