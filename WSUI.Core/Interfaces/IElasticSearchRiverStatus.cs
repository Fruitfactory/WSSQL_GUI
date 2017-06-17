using Elasticsearch.Net;
using Nest;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Data.NamedPipeMessages.Response;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchRiverStatus : IElasticSearchItemsCount
    {
        OFNamedServerResponse GetRiverStatus();
        
    }
}