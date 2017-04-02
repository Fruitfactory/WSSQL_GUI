using System;
using System.Net;
using System.Text;
using Elasticsearch.Net.Serialization;
using Microsoft.Practices.Unity;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Interfaces;
using OF.Core.Logger;
using RestSharp;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticSeachIndexOutlookItemsClient : OFElasticSearchClientBase, IElasticSearchIndexOutlookItemsClient
    {

        private RestClient _restClient = new RestClient("http://localhost:8080");

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
                var request = new RestRequest("parse/items", Method.POST);
                request.AddBody(str);
                var response = _restClient.Execute(request);
                return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return false;
        }
    }
}