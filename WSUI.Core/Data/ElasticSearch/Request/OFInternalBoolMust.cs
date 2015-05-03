using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Request.Base;
using OF.Core.Enums;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFInternalBoolMust<T> : OFBaseInternalBool<T> where T : class, new()
    {
        public OFInternalBoolMust()
            :base()
        {
            _bool.Add("must",new List<T>());
        }

        public override void AddCondition(T condition)
        {
            _bool["must"].Add(condition);
        }
    }
}