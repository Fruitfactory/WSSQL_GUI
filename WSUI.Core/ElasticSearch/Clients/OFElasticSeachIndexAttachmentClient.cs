using System;
using System.Collections.Generic;
using Elasticsearch.Net.Serialization;
using Microsoft.Practices.Unity;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Core.ElasticSearch.Clients
{
    public class OFElasticSeachIndexAttachmentClient : OFElasticSearchClientBase, IElasticSearchIndexAttachmentClient
    {

        [InjectionConstructor]
        public OFElasticSeachIndexAttachmentClient()
        {
            
        }

        public void SendAttachmentToIndex(OFAttachmentIndexingContainer attachmentContainer)
        {
            try
            {
                var attachmentsList = Serializer.Serialize(attachmentContainer, SerializationFormatting.Indented);
                Raw.IndexPut("_river", DefaultInfrastructureName, "indexattachment", attachmentsList);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }
    }
}