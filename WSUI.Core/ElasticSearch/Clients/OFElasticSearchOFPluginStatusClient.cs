using Elasticsearch.Net.Serialization;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Enums;
using OF.Core.Interfaces;

namespace OF.Core.ElasticSearch.Clients
{
    public class OFElasticSearchOFPluginStatusClient : OFElasticSearchClientBase, IElasticSearchOFPluginStatusClient
    {
        public void OFPluginStatus(OFPluginStatus status)
        {
            var container = new OFPluginStatusContainer(){Status = status};
            var body = Serializer.Serialize(container, SerializationFormatting.Indented);

            Raw.IndexPut("_river", DefaultInfrastructureName, "client", body);
        }
    }
}