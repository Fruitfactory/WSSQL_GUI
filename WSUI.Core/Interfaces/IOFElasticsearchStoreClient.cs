using System.Collections.Generic;
using OF.Core.Data.ElasticSearch;

namespace OF.Core.Interfaces
{
    public interface IOFElasticsearchStoreClient
    {
        bool SaveStore(OFStore Store);
        bool DeleteStore(OFStore Store);
        IEnumerable<OFStore> GetStores();
    }
}