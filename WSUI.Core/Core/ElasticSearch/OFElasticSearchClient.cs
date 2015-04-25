using System;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Practices.Unity;
using Nest;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;
using Exception = System.Exception;

namespace OF.Core.Core.ElasticSearch
{
    public class OFElasticSearchClient : IDisposable, IElasticSearchInitializationIndex
    {
        public static readonly string ElasticSearchHost = "http://localhost:9200";
        public static readonly string DefaultIndexName = "outlookfinder";


        private static readonly String WARM_NAME_CONTACT = "warm_contact";
        private static readonly String WARM_NAME_EMAIL = "warm_email";
        private static readonly String WARM_NAME_ATTACHMENT = "warm_attachment";

        private ElasticClient _internalElasticClient;

        [InjectionConstructor]
        public OFElasticSearchClient()
            : this(ElasticSearchHost)
        {
        }

        public OFElasticSearchClient(string host)
            : this(host, DefaultIndexName)
        {
        }

        public OFElasticSearchClient(string host, string defaultIndexName)
        {
            Init(host, defaultIndexName);
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
                OFLogger.Instance.LogError(ex.Message);
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
                Raw.IndexPut("_river", OFElasticSearchClient.DefaultIndexName, "_meta", bodyRequest);
            }
            catch (Exception exception)
            {
                OFLogger.Instance.LogError(exception.Message);
            }

        }

        public ElasticsearchResponse<OFStatusResponse> GetIndexingProgress()
        {
            return Raw.Get<OFStatusResponse>("_river", DefaultIndexName, "status");
        }

        public void CreateWarms()
        {
            try
            {
                ElasticClient.PutWarmer(WARM_NAME_CONTACT, wd =>
                    wd
                   .Type<OFContact>().Index(DefaultIndexName)
                   .Search<OFContact>(s => s.From(0).Size(10)));

                ElasticClient.PutWarmer(WARM_NAME_EMAIL, wd =>
                   wd
                       .Type<OFEmail>().Index(DefaultIndexName)
                       .Search<OFEmail>(s => s.From(0).Size(10)));
                ElasticClient.PutWarmer(WARM_NAME_ATTACHMENT, wd =>
                wd
                    .Type<OFAttachmentContent>().Index(DefaultIndexName)
                    .Search<OFAttachmentContent>(s => s.From(0).Size(10)));
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }

        }

        public void WarmUp()
        {
            Task.Factory.StartNew(WarmUpAll);
        }

        public void Dispose()
        {
            if (ElasticClient != null)
            {
                ElasticClient = null;
            }
            GC.SuppressFinalize(this);
        }

        private void WarmUpAll()
        {
            WarmUpContact();
            WarmUpEmail();
            WarmUpAttachment();
        }

        private void WarmUpContact()
        {
            try
            {
                var result = ElasticClient.Search<OFContact>(s => s.From(0).Size(10));
                if (result.IsNotNull() && result.Documents.Any())
                {
                    OFLogger.Instance.LogInfo("Warm Up Contact: {0} contacts", result.Documents.Count());
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogInfo(ex.Message);
            }
        }

        private void WarmUpEmail()
        {
            try
            {
                var result = ElasticClient.Search<OFEmail>(s => s.From(0).Size(10));
                if (result.IsNotNull() && result.Documents.Any())
                {
                    OFLogger.Instance.LogInfo("Warm Up EMail: {0} emails", result.Documents.Count());
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void WarmUpAttachment()
        {
            try
            {
                var result = ElasticClient.Search<OFAttachmentContent>(s => s.From(0).Size(10));
                if (result.IsNotNull() && result.Documents.Any())
                {
                    OFLogger.Instance.LogInfo("Warm Up attachment: {0} attachments", result.Documents.Count());
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }

        }

    }
}