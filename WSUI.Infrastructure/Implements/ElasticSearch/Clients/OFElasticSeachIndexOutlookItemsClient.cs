using System;
using System.Text;
using Elasticsearch.Net.Serialization;
using Microsoft.Practices.Unity;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticSeachIndexOutlookItemsClient : OFElasticSearchClientBase, IElasticSearchIndexOutlookItemsClient
    {

        [InjectionConstructor]
        public OFElasticSeachIndexOutlookItemsClient()
        {
            
        }

        public bool SendOutlookItemsToIndex(OFOutlookItemsIndexingContainer outlookItemsContainer)
        {
            try
            {
                var attachmentsList = Serializer.Serialize(outlookItemsContainer, SerializationFormatting.Indented);
                var str = Encoding.UTF8.GetString(attachmentsList);
                var response = Raw.IndexPut("_river", DefaultInfrastructureName, "indexattachment", attachmentsList);
                return response.HttpStatusCode == 200;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return false;
        }

        public bool SendOutlookItemsCount(int countItems)
        {
            try
            {
                var body = new {count = countItems};
                var serialized = Serializer.Serialize(body, SerializationFormatting.Indented);
                var respone = Raw.IndexPut("_river", DefaultInfrastructureName, "countitems", serialized);
                return respone.HttpStatusCode == 200;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return false;
        }
    }
}