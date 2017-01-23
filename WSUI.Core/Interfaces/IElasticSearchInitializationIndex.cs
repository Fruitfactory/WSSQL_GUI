using System.Collections.Generic;
using Elasticsearch.Net;
using Nest;
using OF.Core.Data.ElasticSearch.Response;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchInitializationIndex : IElasticSearchRiverStatus
    {
        IExistsResponse IndexExists(string name);
        void CreateInfrastructure();
        ElasticsearchResponse<OFStatusResponse> GetIndexingProgress();

        void WarmUp();

        void CheckAndCreateWarms();
    }
}