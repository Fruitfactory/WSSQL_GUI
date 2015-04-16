using System.IO.Packaging;
using Newtonsoft.Json;
using OF.Core.Enums;

namespace OF.Core.Data.ElasticSearch.Response
{
    public class OFStatusItem
    {
        
        public string Name { get; set; }

        public int Count { get; set; }

        public int Processing { get; set; }

        public PstReaderStatus Status { get; set; }

        public string Folder { get; set; }
    }
}