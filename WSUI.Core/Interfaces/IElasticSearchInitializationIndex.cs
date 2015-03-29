using Elasticsearch.Net;
using Nest;
using WSUI.Core.Data.ElasticSearch.Response;

namespace WSUI.Core.Interfaces
{
    public interface IElasticSearchInitializationIndex
    {
        INestSerializer Serializer { get; }
        IElasticsearchClient Raw { get; }
        IExistsResponse IndexExists(string name);
        void CreateIndex(byte[] bodyRequest);
        ElasticsearchResponse<WSUIStatusResponse> GetIndexingProgress();
    }
}