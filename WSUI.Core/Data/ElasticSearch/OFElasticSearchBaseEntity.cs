using System;
using OF.Core.Interfaces;

namespace OF.Core.Data.ElasticSearch
{
    public class OFElasticSearchBaseEntity : IElasticSearchObject
    {

        public OFElasticSearchBaseEntity()
        {
            IndexedDate = DateTime.Now;
        }

        public string Entryid
        {
            get; set;
        }

        public DateTime IndexedDate
        {
            get;
        }
    }
}