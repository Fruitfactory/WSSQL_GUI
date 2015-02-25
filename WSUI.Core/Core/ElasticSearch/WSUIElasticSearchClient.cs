using System;
using System.Security.Permissions;
using Microsoft.Office.Interop.Outlook;
using Nest;
using WSUI.Core.Logger;
using Exception = System.Exception;

namespace WSUI.Core.Core.ElasticSearch
{
    public class WSUIElasticSearchClient : IDisposable
    {
        public static readonly string ElasticSearchHost = "http://localhost:9200";
        public static readonly string DefaultIndexName = "outlookfinder";

        private ElasticClient _internalElasticClient;

        public WSUIElasticSearchClient()
            :this(ElasticSearchHost)
        {
        }

        public WSUIElasticSearchClient(string host)
            :this(host,DefaultIndexName)
        {
            
        }

        public WSUIElasticSearchClient(string host, string defaultIndexName)
        {
            Init(host,defaultIndexName);
        }

        private void Init(string host, string defaultIndexName)
        {
            try
            {
                var node = new Uri(host);
                var settings = new ConnectionSettings(node, defaultIndexName);
                _internalElasticClient = new ElasticClient(settings);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
        }

        public ElasticClient ElasticClient
        {
            get { return _internalElasticClient; }
        }


        public void Dispose()
        {   
        }
    }
}