using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;

namespace OF.Core.Data.ElasticSearch.Response
{
    public class OFStatusResponse : IElasticsearchResponse
    {
        public OFStatusItem[] Items { get; set; }

        public bool TryGetServerErrorReason(out string reason)
        {
	        throw new System.NotImplementedException();
        }

        public IApiCallDetails ApiCall { get; set; }
    }
}