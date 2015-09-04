using Elasticsearch.Net;
using Nest;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Extensions;
using OF.Core.Interfaces;

namespace OF.Core.ElasticSearch.Clients
{
    public class OFElasticRiverStatus : OFElasticSearchClientBase,IElasticSearchRiverStatus
    {

        public OFElasticRiverStatus()
        {   
        }

        public ElasticsearchResponse<OFRiverStatusInfo> GetRiverStatus()
        {
            return Raw.Get<OFRiverStatusInfo>("_river", DefaultInfrastructureName, "pstriverstatus");
        }

        public IndexStatus GetIndexStatus(string indexName)
        {
            var status = ElasticClient.Status(new IndicesStatusRequest());
            if (status.IsNotNull() && status.Indices.IsNotNull()  && status.Indices.ContainsKey(indexName))
            {
                return status.Indices[indexName];
            }
            return null;
        }

        public long GetTypeCount<T>() where T : class 
        {
            var status = ElasticClient.Count<T>();
            return status.Count;
        }

    }
}