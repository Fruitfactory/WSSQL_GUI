﻿using System;
using Elasticsearch.Net.Serialization;
using Microsoft.Practices.Unity;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticSeachIndexAttachmentClient : OFElasticSearchClientBase, IElasticSearchIndexAttachmentClient
    {

        [InjectionConstructor]
        public OFElasticSeachIndexAttachmentClient()
        {
            
        }

        public bool SendAttachmentToIndex(OFAttachmentIndexingContainer attachmentContainer)
        {
            try
            {
                var attachmentsList = Serializer.Serialize(attachmentContainer, SerializationFormatting.Indented);
                var response = Raw.IndexPut("_river", DefaultInfrastructureName, "indexattachment", attachmentsList);
                return response.HttpStatusCode == 200;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return false;
        }
    }
}