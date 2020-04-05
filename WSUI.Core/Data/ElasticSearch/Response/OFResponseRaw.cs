using Elasticsearch.Net;

namespace OF.Core.Data.ElasticSearch.Response
{
    public class OFResponseRaw<T> : IElasticsearchResponse where T : class 
    {
        public long took { get; set; }

        public bool timed_out { get; set; }

        public OFShardsRaw _shards { get; set; }

        public OFHitsRaw<T> hits { get; set; }

        public bool TryGetServerErrorReason(out string reason)
        {
	        throw new System.NotImplementedException();
        }

        public IApiCallDetails ApiCall { get; set; }
    }
}