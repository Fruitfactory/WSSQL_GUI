using Nest;

namespace OF.Core.Data.ElasticSearch
{
    [ElasticType(Name = "store")]
    public class OFStore
    {
        public string Name { get; set; }

        public string Storeid { get; set; }
    }
}