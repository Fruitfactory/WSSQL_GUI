using System.Collections.Generic;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFInternalBoolShould<T> : OFBaseInternalBool<T> where T : class , new()
    {
        public OFInternalBoolShould()
            :base()
        {
            _bool.Add("should",new List<T>());
        }

        public override void AddCondition(T condition)
        {
            _bool["should"].Add(condition);
        }
    }
}