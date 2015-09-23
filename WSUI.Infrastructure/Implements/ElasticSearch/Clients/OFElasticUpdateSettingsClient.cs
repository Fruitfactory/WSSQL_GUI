using Elasticsearch.Net.Serialization;
using OF.Core;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticUpdateSettingsClient : OFElasticSearchClientBase, IElasticUpdateSettingsClient
    {
        public void UpdateSettings(OFRiverMeta settings)
        {
            if (settings.IsNull())
            {
                return;
            }
            var body = Serializer.Serialize(settings, SerializationFormatting.Indented);

            var response = Raw.IndexPut("_river", DefaultInfrastructureName, "_meta", body);

            OFObjectJsonSaveReadHelper.Instance.Save(settings, GlobalConst.SettingsRiverFile);

            OFLogger.Instance.LogDebug("Update River...");
            OFLogger.Instance.LogDebug("Status: {0}  Success: {1}", response.HttpStatusCode, response.Success);
        }

    }
}