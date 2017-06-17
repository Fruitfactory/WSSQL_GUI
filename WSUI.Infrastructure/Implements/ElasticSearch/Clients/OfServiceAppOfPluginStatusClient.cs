using System;
using Elasticsearch.Net;
using OF.Core;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.NamedPipeMessages;
using OF.Core.Enums;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Infrastructure.NamedPipes;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OfServiceAppOfPluginStatusClient : IServiceAppOFPluginStatusClient
    {
        public void OFPluginStatus(bool status)
        {
            try
            {
                OFNamedPipeClient<OFServiceApplicationMessage> client =
                    new OFNamedPipeClient<OFServiceApplicationMessage>(GlobalConst.ServiceApplicationServer);
                client.Send(new OFServiceApplicationMessage() { MessageType = ofServiceApplicationMessageType.OfPluginState, Payload = status });
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }
    }
}