using System.Collections.Generic;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFBoolMust<T> where T : class
    {
        public List<T> must { get; set; }

        public OFBoolMust()
        {
            must = new List<T>();
        }
    }
}