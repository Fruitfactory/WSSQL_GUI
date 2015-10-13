using Nest;
using Newtonsoft.Json;

namespace OF.Core.Data.ElasticSearch.Response
{
    public class OFStatusResponse
    {
        public OFStatusItem[] Items { get; set; }         
    }
}