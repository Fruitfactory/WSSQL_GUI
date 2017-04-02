using System.Collections.Generic;
using OF.Core.Data.ElasticSearch;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchIndexOutlookItemsClient
    {
        bool SendOutlookItemsToIndex(OFOutlookItemsIndexingContainer outlookItemsContainer);
    }
}