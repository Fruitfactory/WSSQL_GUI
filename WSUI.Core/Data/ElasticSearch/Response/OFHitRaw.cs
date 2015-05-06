namespace OF.Core.Data.ElasticSearch.Response
{
    public class OFHitRaw<T> where T : class
    {
        public string _index { get; set; }

        public string _type { get; set; }

        public string _id { get; set; }

        public object _score { get; set; }

        public long[] sort { get; set; }

        public T _source { get; set; }
    }
}