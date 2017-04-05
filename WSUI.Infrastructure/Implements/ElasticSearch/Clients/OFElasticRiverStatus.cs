using Elasticsearch.Net;
using Nest;
using OF.Core;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Data.NamedPipeMessages;
using OF.Core.Data.NamedPipeMessages.Response;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Infrastructure.NamedPipes;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticRiverStatus : OFElasticSearchClientBase,IElasticSearchRiverStatus
    {

        public OFElasticRiverStatus()
        {   
        }

        public OFNamedServerResponse GetRiverStatus()
        {
            OFNamedPipeClient<OFServiceApplicationMessage> client = new OFNamedPipeClient<OFServiceApplicationMessage>(GlobalConst.ServiceApplicationServer);
            var response =
                client.Send(new OFServiceApplicationMessage()
                {
                    MessageType = ofServiceApplicationMessageType.ControllerStatus
                });
            return response;
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