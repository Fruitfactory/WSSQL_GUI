namespace OF.Core.Data.ElasticSearch.Request.Base
{
    public class OFBaseCondition<T> where T :  class
    {
        public string Key { get; protected set; }

        public T Value { get; set; }
    }
}