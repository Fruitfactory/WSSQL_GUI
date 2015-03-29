using Nest;
using Newtonsoft.Json;

namespace WSUI.Core.Data.ElasticSearch.Response
{
    public class WSUIStatusResponse
    {
        public WSUIStatusItem[] Items { get; set; }         
    }
}