using Nest;

namespace OF.Core.Data.ElasticSearch
{
    [ElasticsearchType(Name = "store")]
    public class OFStore
    {
        public string Name { get; set; }

        public string Storeid { get; set; }
    }
}