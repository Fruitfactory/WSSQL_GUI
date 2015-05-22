using System.Collections.Generic;

namespace OF.Core.Data.ElasticSearch.Request.Email
{
    public class OFSortDateCreated
    {
        public Dictionary<string,object> datereceived { get; set; }

        public OFSortDateCreated()
        {
            datereceived = new Dictionary<string, object>();
            datereceived.Add("order","desc");
        }
    }
}