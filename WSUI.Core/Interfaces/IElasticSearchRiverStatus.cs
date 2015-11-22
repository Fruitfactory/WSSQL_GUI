using Elasticsearch.Net;
using Nest;
using OF.Core.Data.ElasticSearch.Response;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchRiverStatus : IElasticSearchItemsCount
    {
        ElasticsearchResponse<OFRiverStatusInfo> GetRiverStatus();
        IndexStatus GetIndexStatus(string indexName);
        
    }
}