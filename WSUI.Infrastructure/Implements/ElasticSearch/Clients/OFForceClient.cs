using System;
using OF.Core;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.NamedPipeMessages;
using OF.Core.Enums;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Infrastructure.NamedPipes;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFForceClient : IForceClient
    {
        public void Force()
        {
            try
            {
                OFNamedPipeClient<OFServiceApplicationMessage> client =
                    new OFNamedPipeClient<OFServiceApplicationMessage>(GlobalConst.ServiceApplicationServer);
                client.Send(new OFServiceApplicationMessage(){MessageType  = ofServiceApplicationMessageType.ForceIndexing, Payload = new OFIsForcedMessage() {IsForced = true}});
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }
    }
}