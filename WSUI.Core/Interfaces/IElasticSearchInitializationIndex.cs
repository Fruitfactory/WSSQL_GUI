using System.Collections.Generic;
using Elasticsearch.Net;
using Nest;
using OF.Core.Data.ElasticSearch.Response;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchInitializationIndex : IElasticSearchRiverStatus
    {
        ExistsResponse IndexExists(string name);
        void CreateInfrastructure();
        OFStatusResponse GetIndexingProgress();

        void WarmUp();
        
    }
}