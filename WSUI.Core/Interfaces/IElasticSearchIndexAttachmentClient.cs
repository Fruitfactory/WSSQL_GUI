using System.Collections.Generic;
using OF.Core.Data.ElasticSearch;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchIndexAttachmentClient
    {
        void SendAttachmentToIndex(OFAttachmentIndexingContainer attachmentContainer);
    }
}