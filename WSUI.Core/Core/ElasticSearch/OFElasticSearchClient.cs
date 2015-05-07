using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Elasticsearch.Net.Serialization;
using Microsoft.Practices.Unity;
using Nest;
using Newtonsoft.Json;
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
        private const string Somevalue = "somevalue";
        public static readonly string ElasticSearchHost = "http://localhost:9200";
        public static readonly string DefaultIndexName = "outlookfinder";


        private static readonly String WARM_NAME_CONTACT = "warm_contact";
        private static readonly String WARM_NAME_EMAIL = "warm_email";
        private static readonly String WARM_NAME_ATTACHMENT = "warm_attachment";

        private static readonly int HITS_INDEX = 2;
        private static readonly int TOTAL_INDEX = 0;

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

        public IRawSearchResult<T> RawSearch<T>(object body) where T : class, new()
        {
            byte[] bodyBytes = Serializer.Serialize(body, SerializationFormatting.Indented);
            IEnumerable<T> listResult = null;
            int took = 0;
            int total = 0;
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                var result = Raw.Search<byte[]>(DefaultIndexName, GetSearchType(typeof(T)), bodyBytes);

                watch.Stop();
                OFLogger.Instance.LogInfo("Search (Send request to ES and retrieve response): {0}ms",watch.ElapsedMilliseconds);

                Stopwatch watchParsing = new Stopwatch();
                watchParsing.Start();

                using (var stream = new MemoryStream(result.Response))
                using (var reader = new StreamReader(stream))
                {
                    var rawResult = JsonSerializer.Create().Deserialize(reader, typeof(OFResponseRaw<T>)) as OFResponseRaw<T>;
                    took = (int)rawResult.took;
                    total = rawResult.hits.total;
                    listResult = rawResult.hits.hits.Select(h => h._source);
                }
                watchParsing.Stop();
                OFLogger.Instance.LogInfo("Parsing dynamic response: {0}ms, Diff: {1}ms", watchParsing.ElapsedMilliseconds, Math.Abs(watchParsing.ElapsedMilliseconds - watch.ElapsedMilliseconds));

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
            
            return new OFRawSearchResponse<T>(took,total,listResult);
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
                ElasticClient.PutWarmer("warm_contact_1", wd =>
                    wd.Type<OFContact>().Index(DefaultIndexName)
                        .Search<OFContact>(s => s.From(0).Size(10).Query(q =>
                            q.Bool(bd => bd.Should(t => t.Term(c => c.Firstname, Somevalue),
                                t => t.Term(c => c.Lastname, Somevalue),
                                t => t.Term(c => c.Emailaddress1, Somevalue),
                                t => t.Term(c => c.Emailaddress2, Somevalue),
                                t => t.Term(c => c.Emailaddress3, Somevalue)))))
                    );

                ElasticClient.PutWarmer("warm_email_1", wd =>
                    wd.Type<OFEmail>().Index(DefaultIndexName)
                    .Search<OFEmail>(s => s.From(0).Size(10).Query(q =>
                        q.Term(e => e.Subject, Somevalue))));

                ElasticClient.PutWarmer("warm_email_2", wd =>
                                    wd.Type<OFEmail>().Index(DefaultIndexName)
                                    .Search<OFEmail>(s => s.From(0).Size(10).Query(q =>
                                        q.Term(e => e.Analyzedcontent, Somevalue))));

                ElasticClient.PutWarmer("warm_email_3", wd =>
                                    wd.Type<OFEmail>().Index(DefaultIndexName)
                                    .Search<OFEmail>(s => s.From(0).Size(10).Query( queryDescriptor => queryDescriptor.Bool(bd => bd.Should(
                                                                                                                                qd => qd.Term("to.name", Somevalue),
                                                                                                                                qd => qd.Term("to.address", Somevalue),
                                                                                                                                qd => qd.Term("cc.name", Somevalue),
                                                                                                                                qd => qd.Term("cc.address", Somevalue),
                                                                                                                                qd => qd.Term("fromname", Somevalue),
                                                                                                                                qd => qd.Term("fromaddress", Somevalue)
                                                                                                                                )))));

                ElasticClient.PutWarmer("warm_attachment_1", wd =>
                    wd.Type<OFAttachmentContent>().Index(DefaultIndexName)
                        .Search<OFAttachmentContent>(s => s.From(0).Size(10).Query(q =>
                            q.Term(a => a.Filename, Somevalue))));

                ElasticClient.PutWarmer("warm_attachment_2", wd =>
                    wd.Type<OFAttachmentContent>().Index(DefaultIndexName)
                        .Search<OFAttachmentContent>(s => s.From(0).Size(10).Query(q =>
                            q.Term(a => a.Analyzedcontent, Somevalue))));



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

        private string GetSearchType(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(ElasticTypeAttribute),false);
            string result = "";
            if (attrs != null && attrs.Length > 0)
            {
                var elasticType = attrs[0] as ElasticTypeAttribute;
                result = elasticType.Name;
            }
            return result;
        }

    }
}