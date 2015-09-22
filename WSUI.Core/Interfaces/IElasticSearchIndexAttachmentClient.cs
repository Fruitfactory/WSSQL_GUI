using System.Collections.Generic;
using OF.Core.Data.ElasticSearch;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchIndexAttachmentClient
    {
        bool SendAttachmentToIndex(OFAttachmentIndexingContainer attachmentContainer);
    }
}