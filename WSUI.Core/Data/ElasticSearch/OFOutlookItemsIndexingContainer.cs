using System.Collections.Generic;
using OF.Core.Enums;

namespace OF.Core.Data.ElasticSearch
{
    public class OFOutlookItemsIndexingContainer
    {
        public OFEmail Email { get; set; }

        public IEnumerable<OFAttachmentContent> Attachments { get; set; }

        public OFContact Contact { get; set; }

        public OFOutlookItemsIndexProcess Process { get; set; }
    }
}