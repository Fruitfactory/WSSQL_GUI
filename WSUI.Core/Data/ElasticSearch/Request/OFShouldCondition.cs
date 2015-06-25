using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFShouldCondition<T> : OFBaseCondition<T> where T : class
    {
        public OFShouldCondition()
        {
            Key = "should";
        }
    }
}