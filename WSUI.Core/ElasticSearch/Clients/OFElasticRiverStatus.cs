using Elasticsearch.Net;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Interfaces;

namespace OF.Core.ElasticSearch.Clients
{
    public class OFElasticRiverStatus: OFElasticSearchClientBase,IElasticSearchRiverStatus
    {

        public OFElasticRiverStatus()
        {   
        }
        public ElasticsearchResponse<OFRiverStatusInfo> GetRiverStatus()
        {
            return Raw.Get<OFRiverStatusInfo>("_river", DefaultInfrastructureName, "pstriverstatus");
        }
    }
}