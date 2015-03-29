using System;
using Elasticsearch.Net;
using Microsoft.Practices.Unity;
using Nest;
using WSUI.Core.Data.ElasticSearch.Response;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using Exception = System.Exception;

namespace WSUI.Core.Core.ElasticSearch
{
    public class WSUIElasticSearchClient : IDisposable, IElasticSearchInitializationIndex
    {
        public static readonly string ElasticSearchHost = "http://localhost:9200";
        public static readonly string DefaultIndexName = "outlookfinder";

        private ElasticClient _internalElasticClient;

        [InjectionConstructor]
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
                ElasticClient = new ElasticClient(settings);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
        }

        private ElasticClient ElasticClient
        {
            get { return _internalElasticClient; }
            set { _internalElasticClient = value; }
        }

        public ISearchResponse<T> Search<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searchSelector)
            where T : class
        {
            return ElasticClient.Search<T>(searchSelector);
        }

        public INestSerializer Serializer
        {
            get { return ElasticClient.Serializer; }
        }

        public IElasticsearchClient Raw
        {
            get { return ElasticClient.Raw; }
        }

        public IExistsResponse IndexExists(string name)
        {
            return ElasticClient.IndexExists(name);
        }

        public void CreateIndex(byte[] bodyRequest)
        {
            try
            {
                Raw.IndexPut("_river", WSUIElasticSearchClient.DefaultIndexName, "_meta", bodyRequest);
            }
            catch (Exception exception)
            {
                WSSqlLogger.Instance.LogError(exception.Message);
            }
            
        }

        public ElasticsearchResponse<WSUIStatusResponse> GetIndexingProgress()
        {
            return Raw.Get<WSUIStatusResponse>("_river", DefaultIndexName, "status");
        } 

        public void Dispose()
        {
            if (ElasticClient != null)
            {
                ElasticClient = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}