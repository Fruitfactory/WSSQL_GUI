using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace OF.Core.Data.ElasticSearch.Request.Base
{
    public abstract class OFBaseInternalConditions<T> where T : class
    {
        [JsonProperty("bool")]
        public List<KeyValuePair<string,T>> _bool { get; set; }

        protected OFBaseInternalConditions()
        {
            _bool = new List<KeyValuePair<string, T>>();
        }

        protected virtual void AddCondition(string name, T value)
        {
            _bool.Add(new KeyValuePair<string, T>(name,value));
        }

    }
}