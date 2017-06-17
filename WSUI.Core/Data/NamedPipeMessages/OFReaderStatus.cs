using System;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Enums;

namespace OF.Core.Data.NamedPipeMessages
{
    public class OFReaderStatus
    {

        public OFRiverStatus ControllerStatus { get; set; }


        public PstReaderStatus ReaderStatus { get; set; }
        
        public int Count { get; set; }

        public string Folder { get; set; }

        public DateTime LastUpdated { get; set; }
        
    }
}