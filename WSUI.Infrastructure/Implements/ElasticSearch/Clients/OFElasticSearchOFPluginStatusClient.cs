using Elasticsearch.Net;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Enums;
using OF.Core.Interfaces;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticSearchOFPluginStatusClient : OFElasticSearchClientBase, IElasticSearchOFPluginStatusClient
    {
        public void OFPluginStatus(OFPluginStatus status)
        {
            var container = new OFPluginStatusContainer(){Status = status};
            var body = Serializer.SerializeToString(container);

            //TODO: send status to service app
            //Raw.IndexPut("_river", DefaultInfrastructureName, "client", body);
        }
    }
}