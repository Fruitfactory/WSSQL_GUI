using System.Collections.Generic;
using Newtonsoft.Json;

namespace OF.Core.Data.ElasticSearch.Request.Base
{
    public abstract class OFBaseInternalBool<T> where T : class
    {
        [JsonProperty("bool")]
        public Dictionary<string,List<T>> _bool { get; set; }

        protected OFBaseInternalBool()
        {
            _bool = new Dictionary<string, List<T>>();
        }
        public abstract void AddCondition(T condition);
    }
}