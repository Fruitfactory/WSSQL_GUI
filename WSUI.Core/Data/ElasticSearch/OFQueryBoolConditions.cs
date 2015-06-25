using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Request;

namespace OF.Core.Data.ElasticSearch
{
    public class OFQueryBoolConditions
    {
        [JsonProperty("bool")]
        public OFConditionCollection _bool { get; private set; }

        public OFQueryBoolConditions()
        {
            _bool = new OFConditionCollection();
        }
    }
}