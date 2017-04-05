using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Elasticsearch.Net.Serialization;
using Microsoft.Practices.Unity;
using Nest;
using Newtonsoft.Json;
using OF.Core;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Data.NamedPipeMessages;
using OF.Core.Data.NamedPipeMessages.Response;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Infrastructure.NamedPipes;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticInitializingClient : OFElasticSearchClientBase, IElasticSearchInitializationIndex
    {

        private const string Somevalue = "outlook";

        private static readonly String WARM_NAME_CONTACT = "warm_contact";
        private static readonly String WARM_NAME_EMAIL = "warm_email";
        private static readonly String WARM_NAME_ATTACHMENT = "warm_attachment";

        private static readonly int WARM_UP_SIZER = 1000;


        [InjectionConstructor()]
        public OFElasticInitializingClient()
        {
        }

        public void CreateInfrastructure()
        {
            try
            {
                if (CreateIndex())
                {
                    CreateRiver();
                    CreateWarms();
                }
            }
            catch (Exception exception)
            {
                OFLogger.Instance.LogError("Creating Infrastructure was failed... Error below:");
                OFLogger.Instance.LogError(exception.ToString());
            }

        }

        public ElasticsearchResponse<OFStatusResponse> GetIndexingProgress()
        {
            return Raw.Get<OFStatusResponse>("_river", DefaultInfrastructureName, "status");
        }

        public OFNamedServerResponse GetRiverStatus()
        {

            OFNamedPipeClient<OFServiceApplicationMessage> client = new OFNamedPipeClient<OFServiceApplicationMessage>(GlobalConst.ServiceApplicationServer);
            var response =
                client.Send(new OFServiceApplicationMessage()
                {
                    MessageType = ofServiceApplicationMessageType.ControllerStatus
                });
            return response;

        }

        public IndexStatus GetIndexStatus(string indexName)
        {
            throw new NotImplementedException();
        }

        public long GetTypeCount<T>() where T : class
        {
            var status = ElasticClient.Count<T>();
            return status.Count;
        }

        public void CheckAndCreateWarms()
        {
            var response = ElasticClient.GetWarmer("warm_contact_1");
            if (response.Indices != null && !response.Indices.Any())
            {
                CreateWarms();
            }
        }

        public void WarmUp()
        {
            Task.Factory.StartNew(WarmUpAll);
        }

        private void WarmUpAll()
        {
            ElasticClient.Warmup();
            WarmUpContact();
            WarmUpEmail();
            WarmUpAttachment();
        }

        private void WarmUpContact()
        {
            try
            {
                var value = "a";
                var result = ElasticClient.Search<OFContact>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                            q.Bool(bd => bd.Should(t => t.Term(c => c.Firstname, value),
                                t => t.Term(c => c.Lastname, value),
                                t => t.Term(c => c.Emailaddress1, value),
                                t => t.Term(c => c.Emailaddress2, value),
                                t => t.Term(c => c.Emailaddress3, value)))));
                if (result.IsNotNull() && result.Documents.Any())
                {
                    OFLogger.Instance.LogDebug("Warm Up Contact: {0} contacts", result.Documents.Count());
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogInfo(ex.ToString());
            }
        }

        private void WarmUpEmail()
        {
            try
            {

                var value = "outlook";
                var email = "@";
                var result = ElasticClient.Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                        q.Term(e => e.Subject, value)));
                result = ElasticClient.Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                                        q.Term(e => e.Analyzedcontent, value)));
                result = ElasticClient.Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(queryDescriptor => queryDescriptor.Bool(bd => bd.Should(
                                                                                                                                qd => qd.Term("to.name", email),
                                                                                                                                qd => qd.Term("to.address", email),
                                                                                                                                qd => qd.Term("cc.name", email),
                                                                                                                                qd => qd.Term("cc.address", email),
                                                                                                                                qd => qd.Term("fromname", email),
                                                                                                                                qd => qd.Term("fromaddress", email)
                                                                                                                                ))));
                if (result.IsNotNull() && result.Documents.Any())
                {
                    OFLogger.Instance.LogDebug("Warm Up EMail: {0} emails", result.Documents.Count());
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void WarmUpAttachment()
        {
            try
            {
                var image = "png";
                var content = "a";
                var result = ElasticClient.Search<OFAttachmentContent>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                            q.Term(a => a.Filename, image)));
                result = ElasticClient.Search<OFAttachmentContent>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                            q.Term(a => a.Analyzedcontent, content)));
                if (result.IsNotNull() && result.Documents.Any())
                {
                    OFLogger.Instance.LogDebug("Warm Up attachment: {0} attachments", result.Documents.Count());
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }

        }

        private bool CreateIndex()
        {
            var response = ElasticClient.CreateIndex(DefaultInfrastructureName, c => c.NumberOfShards(1).NumberOfReplicas(0).Analysis(
                ad =>
                {
                    ad.TokenFilters(d =>
                    {
                        d.Add("shingle_filter", new ShingleTokenFilter() {MinShingleSize = 2, MaxShingleSize = 10});
                        d.Add("edgeNGram", new EdgeNGramTokenFilter() {MinGram = 2, MaxGram = 15});
                        return d;
                    });
                    ad.Analyzers(
                        d =>
                            d.Add("shingle_analyzer",
                                new CustomAnalyzer()
                                {
                                    Tokenizer = "whitespace",
                                    Filter = new List<string>() { "lowercase", "shingle_filter"}
                                }));
                    return ad;
                }
                )
                .AddMapping<OFEmail>(m => m.MapFromAttributes())
                .AddMapping<OFContact>(m => m.MapFromAttributes())
                .AddMapping<OFAttachmentContent>(m => m.MapFromAttributes())
                );
            OFLogger.Instance.LogDebug("Create Index...");
            OFLogger.Instance.LogDebug("Status: {0}  Success: {1}", response.ConnectionStatus.HttpStatusCode, response.ConnectionStatus.Success);

            return response.ConnectionStatus.HttpStatusCode == 200;

        }

        private void CreateRiver()
        {
            var riverMeta = new OFRiverMeta(DefaultInfrastructureName);

            OFObjectJsonSaveReadHelper.Instance.Save(riverMeta, GlobalConst.SettingsRiverFile);

            OFLogger.Instance.LogDebug("Create Settings for service contoroller...");
        }

        private void CreateWarms()
        {
            try
            {
                OFLogger.Instance.LogDebug("Create warms...");
                var value = "a";
                ElasticClient.PutWarmer("warm_contact_1", wd =>
                    wd.Type<OFContact>().Index(DefaultInfrastructureName)
                        .Search<OFContact>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                            q.Bool(bd => bd.Should(t => t.Term(c => c.Firstname, value),
                                t => t.Term(c => c.Lastname, value),
                                t => t.Term(c => c.Emailaddress1, value),
                                t => t.Term(c => c.Emailaddress2, value),
                                t => t.Term(c => c.Emailaddress3, value)))))
                    );

                var email1 = "outlook";
                var email2 = "@";
                ElasticClient.PutWarmer("warm_email_1", wd =>
                    wd.Type<OFEmail>().Index(DefaultInfrastructureName)
                    .Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                        q.Term(e => e.Subject, email1))));

                ElasticClient.PutWarmer("warm_email_2", wd =>
                                    wd.Type<OFEmail>().Index(DefaultInfrastructureName)
                                    .Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                                        q.Term(e => e.Analyzedcontent, email1))));

                ElasticClient.PutWarmer("warm_email_3", wd =>
                                    wd.Type<OFEmail>().Index(DefaultInfrastructureName)
                                    .Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(queryDescriptor => queryDescriptor.Bool(bd => bd.Should(
                                                                                                                                qd => qd.Term("to.name", email2),
                                                                                                                                qd => qd.Term("to.address", email2),
                                                                                                                                qd => qd.Term("cc.name", email2),
                                                                                                                                qd => qd.Term("cc.address", email2),
                                                                                                                                qd => qd.Term("fromname", email2),
                                                                                                                                qd => qd.Term("fromaddress", email2)
                                                                                                                                )))));
                var image = "png";
                var content = "a";
                ElasticClient.PutWarmer("warm_attachment_1", wd =>
                    wd.Type<OFAttachmentContent>().Index(DefaultInfrastructureName)
                        .Search<OFAttachmentContent>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                            q.Term(a => a.Filename, image))));

                ElasticClient.PutWarmer("warm_attachment_2", wd =>
                    wd.Type<OFAttachmentContent>().Index(DefaultInfrastructureName)
                        .Search<OFAttachmentContent>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                            q.Term(a => a.Analyzedcontent, content))));



                ElasticClient.PutWarmer(WARM_NAME_CONTACT, wd =>
                    wd
                   .Type<OFContact>().Index(DefaultInfrastructureName)
                   .Search<OFContact>(s => s.From(0).Size(WARM_UP_SIZER)));

                ElasticClient.PutWarmer(WARM_NAME_EMAIL, wd =>
                   wd
                       .Type<OFEmail>().Index(DefaultInfrastructureName)
                       .Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER)));
                ElasticClient.PutWarmer(WARM_NAME_ATTACHMENT, wd =>
                wd
                    .Type<OFAttachmentContent>().Index(DefaultInfrastructureName)
                    .Search<OFAttachmentContent>(s => s.From(0).Size(WARM_UP_SIZER)));
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }

        }



    }
}