using System.Collections.Generic;
using OF.Core.Enums;

namespace OF.Core.Data.ElasticSearch
{
    public class OFAttachmentIndexingContainer
    {
        public IEnumerable<OFAttachmentContent> Attachments { get; set; }

        public OFAttachmentIndexProcess Process { get; set; }
    }
}