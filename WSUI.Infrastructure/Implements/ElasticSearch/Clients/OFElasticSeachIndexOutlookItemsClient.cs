using System;
using System.Net;
using System.Text;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Interfaces;
using OF.Core.Logger;
using RestSharp;
using Newtonsoft.Json.Serialization;
using OF.Core.JsonSettings;

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
                var attachmentsList = JsonConvert.SerializeObject(outlookItemsContainer,GetJsonSettings());
                var request = new RestRequest("parse/items", Method.POST);
                request.AddHeader("Accept", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", attachmentsList, ParameterType.RequestBody);
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