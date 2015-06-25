using System.Collections;
using System.Collections.Generic;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFConditionCollection: IEnumerable<OFBaseCondition<object>>
    {
        private List<OFBaseCondition<object>>  list = new List<OFBaseCondition<object>>();

        public void Add(OFBaseCondition<object> condition)
        {
            list.Add(condition);
        }

        public IEnumerator<OFBaseCondition<object>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator(); ;
        }
    }
}