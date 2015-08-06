using System;
using Elasticsearch.Net.Serialization;
using Microsoft.Practices.Unity;
using OF.Core.Core.ElasticSearch;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Core.ElasticSearch.Clients
{
    public class OFElasticTrackingClient : OFElasticSearchClientBase, IElasticTrackingClient
    {
        [InjectionConstructor]
        public OFElasticTrackingClient()
        {
            
        }

        public void SetUserActivityTime(int seconds)
        {
            try
            {
                var idleTime = new
                {
                    idle_time = seconds
                };
                var body = Serializer.Serialize(idleTime, SerializationFormatting.Indented);
                var response = Raw.IndexPut("_river", DefaultInfrastructureName, "useractivity", body);
                //OFLogger.Instance.LogDebug("Set User Activity: {0}l Response: Code => {1}; Success => {2}", seconds, response.HttpStatusCode, response.Success);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        } 
    }
}