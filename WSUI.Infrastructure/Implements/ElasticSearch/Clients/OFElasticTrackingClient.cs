using System;
using Elasticsearch.Net.Serialization;
using Microsoft.Practices.Unity;
using OF.Core.Core.ElasticSearch;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
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
                Raw.IndexPut("_river", DefaultInfrastructureName, "useractivity", body);
                
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        } 
    }
}