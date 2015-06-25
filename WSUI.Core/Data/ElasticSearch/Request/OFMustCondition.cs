using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFMustCondition<T> : OFBaseCondition<T> where T : class
    {
        public OFMustCondition()
        {
            Key = "must";
        }
    }
}