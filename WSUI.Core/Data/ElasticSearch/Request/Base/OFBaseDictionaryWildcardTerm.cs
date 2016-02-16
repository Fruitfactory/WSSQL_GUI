using System.Collections.Generic;

namespace OF.Core.Data.ElasticSearch.Request.Base
{
    public abstract class OFBaseDictionaryWildcardTerm : OFBaseTerm
    {
        public Dictionary<string, Dictionary<string, object>> wildcard { get; set; }

        protected OFBaseDictionaryWildcardTerm()
        {
            wildcard = new Dictionary<string, Dictionary<string, object>>();
        }

        protected abstract string GetKey();

        public override void SetValue(object value)
        {
            wildcard.Add(GetKey(), new Dictionary<string, object>());
            wildcard[GetKey()].Add("value", string.Format("*{0}*",value));
        }
    }
}