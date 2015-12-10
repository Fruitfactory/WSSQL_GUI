using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Elasticsearch.Net.Serialization;
using Microsoft.Practices.Unity;
using Nest;
using OF.Core;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticInitializingClient : OFElasticSearchClientBase, IElasticSearchInitializationIndex
    {

        private const string Somevalue = "somevalue";

        private static readonly String WARM_NAME_CONTACT = "warm_contact";
        private static readonly String WARM_NAME_EMAIL = "warm_email";
        private static readonly String WARM_NAME_ATTACHMENT = "warm_attachment";

        private static readonly int WARM_UP_SIZER = 100;
        

        [InjectionConstructor()]
        public OFElasticInitializingClient()
        {
        }

        public void CreateInfrastructure(IEnumerable<string> listOfFiles)
        {
            try
            {
                if (CreateIndex())
                {
                    CreateRiver(listOfFiles);
                }
            }
            catch (Exception exception)
            {
                OFLogger.Instance.LogError(exception.Message);
            }

        }

        public ElasticsearchResponse<OFStatusResponse> GetIndexingProgress()
        {
            return Raw.Get<OFStatusResponse>("_river", DefaultInfrastructureName, "status");
        }

        public ElasticsearchResponse<OFRiverStatusInfo> GetRiverStatus()
        {
            return Raw.Get<OFRiverStatusInfo>("_river", DefaultInfrastructureName, "pstriverstatus");
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


        public void CreateWarms()
        {
            try
            {
                ElasticClient.PutWarmer("warm_contact_1", wd =>
                    wd.Type<OFContact>().Index(DefaultInfrastructureName)
                        .Search<OFContact>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                            q.Bool(bd => bd.Should(t => t.Term(c => c.Firstname, Somevalue),
                                t => t.Term(c => c.Lastname, Somevalue),
                                t => t.Term(c => c.Emailaddress1, Somevalue),
                                t => t.Term(c => c.Emailaddress2, Somevalue),
                                t => t.Term(c => c.Emailaddress3, Somevalue)))))
                    );

                ElasticClient.PutWarmer("warm_email_1", wd =>
                    wd.Type<OFEmail>().Index(DefaultInfrastructureName)
                    .Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                        q.Term(e => e.Subject, Somevalue))));

                ElasticClient.PutWarmer("warm_email_2", wd =>
                                    wd.Type<OFEmail>().Index(DefaultInfrastructureName)
                                    .Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                                        q.Term(e => e.Analyzedcontent, Somevalue))));

                ElasticClient.PutWarmer("warm_email_3", wd =>
                                    wd.Type<OFEmail>().Index(DefaultInfrastructureName)
                                    .Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(queryDescriptor => queryDescriptor.Bool(bd => bd.Should(
                                                                                                                                qd => qd.Term("to.name", Somevalue),
                                                                                                                                qd => qd.Term("to.address", Somevalue),
                                                                                                                                qd => qd.Term("cc.name", Somevalue),
                                                                                                                                qd => qd.Term("cc.address", Somevalue),
                                                                                                                                qd => qd.Term("fromname", Somevalue),
                                                                                                                                qd => qd.Term("fromaddress", Somevalue)
                                                                                                                                )))));

                ElasticClient.PutWarmer("warm_attachment_1", wd =>
                    wd.Type<OFAttachmentContent>().Index(DefaultInfrastructureName)
                        .Search<OFAttachmentContent>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                            q.Term(a => a.Filename, Somevalue))));

                ElasticClient.PutWarmer("warm_attachment_2", wd =>
                    wd.Type<OFAttachmentContent>().Index(DefaultInfrastructureName)
                        .Search<OFAttachmentContent>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                            q.Term(a => a.Analyzedcontent, Somevalue))));



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

        public void WarmUp()
        {
            Task.Factory.StartNew(WarmUpAll);
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
                var result = ElasticClient.Search<OFContact>(s => s.From(0).Size(WARM_UP_SIZER));
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
                var result = ElasticClient.Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER));
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
                var result = ElasticClient.Search<OFAttachmentContent>(s => s.From(0).Size(WARM_UP_SIZER));
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
            var response = ElasticClient.CreateIndex(DefaultInfrastructureName, c => c.NumberOfReplicas(0)
                .NumberOfShards(1));
            OFLogger.Instance.LogDebug("Create Index...");
            OFLogger.Instance.LogDebug("Status: {0}  Success: {1}", response.ConnectionStatus.HttpStatusCode, response.ConnectionStatus.Success);

            return response.ConnectionStatus.HttpStatusCode == 200;

        }

        private void CreateRiver(IEnumerable<string> listOfFiles)
        {
            var riverMeta = new OFRiverMeta(listOfFiles,DefaultInfrastructureName);
            var body = Serializer.Serialize(riverMeta, SerializationFormatting.Indented);

            var response = Raw.IndexPut("_river", DefaultInfrastructureName, "_meta", body);

            OFObjectJsonSaveReadHelper.Instance.Save(riverMeta, GlobalConst.SettingsRiverFile);

            OFLogger.Instance.LogDebug("Create River...");
            OFLogger.Instance.LogDebug("Status: {0}  Success: {1}", response.HttpStatusCode, response.Success);
        }

    }
}