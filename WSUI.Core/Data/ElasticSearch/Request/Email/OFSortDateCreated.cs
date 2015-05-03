using System.Collections.Generic;

namespace OF.Core.Data.ElasticSearch.Request.Email
{
    public class OFSortDateCreated
    {
        public Dictionary<string,object> datecreated { get; set; }

        public OFSortDateCreated()
        {
            datecreated = new Dictionary<string, object>();
            datecreated.Add("order","desc");
        }
    }
}