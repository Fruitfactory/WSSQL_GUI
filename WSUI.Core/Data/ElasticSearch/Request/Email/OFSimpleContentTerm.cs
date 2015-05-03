using System.Collections.Generic;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Email
{
    public class OFSimpleContentTerm : OFBaseTerm
    {
        public Dictionary<string,object> analyzedcontent { get; set; }

        public OFSimpleContentTerm()
        {
            analyzedcontent = new Dictionary<string, object>();
        }

        public override void SetValue(object value)
        {
            analyzedcontent.Add("value",value);
        }
    }
}