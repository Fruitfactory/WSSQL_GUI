using Elasticsearch.Net;
using Nest;
using OF.Core.Data.ElasticSearch.Response;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchInitializationIndex
    {
        INestSerializer Serializer { get; }
        IElasticsearchClient Raw { get; }
        IExistsResponse IndexExists(string name);
        void CreateIndex(byte[] bodyRequest);
        ElasticsearchResponse<OFStatusResponse> GetIndexingProgress();
    }
}