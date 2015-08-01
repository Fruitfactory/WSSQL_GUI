using Elasticsearch.Net;
using OF.Core.Data.ElasticSearch.Response;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchRiverStatus
    {
        ElasticsearchResponse<OFRiverStatusInfo> GetRiverStatus(); 
    }
}