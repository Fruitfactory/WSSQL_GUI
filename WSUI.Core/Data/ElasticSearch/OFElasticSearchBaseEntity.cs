using OF.Core.Interfaces;

namespace OF.Core.Data.ElasticSearch
{
    public class OFElasticSearchBaseEntity : IElasticSearchObject
    {
        public string Entryid
        {
            get; set;
        }
    }
}