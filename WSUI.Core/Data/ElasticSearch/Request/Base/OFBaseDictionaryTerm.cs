using System.Collections.Generic;

namespace OF.Core.Data.ElasticSearch.Request.Base
{
    public abstract class OFBaseDictionaryTerm : OFBaseTerm
    {
        public Dictionary<string,Dictionary<string,object>> term { get; set; }

        protected OFBaseDictionaryTerm()
        {
            term = new Dictionary<string, Dictionary<string, object>>();
        }

        protected abstract string GetKey();

        public override void SetValue(object value)
        {
            term.Add(GetKey(),new Dictionary<string, object>());
            term[GetKey()].Add("value",value);
        }
    }
}