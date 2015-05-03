using System.Collections.Generic;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFBoolShould<T> where T: class
    {
         
        public List<T> should { get; set; }

        public OFBoolShould()
        {
            should = new List<T>();
        }
    }
}