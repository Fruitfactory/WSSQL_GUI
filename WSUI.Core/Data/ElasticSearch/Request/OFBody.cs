namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFBody
    {
        public int from { get; set; }
        public int size { get; set; }
        public object query { get; set; }
    }
}