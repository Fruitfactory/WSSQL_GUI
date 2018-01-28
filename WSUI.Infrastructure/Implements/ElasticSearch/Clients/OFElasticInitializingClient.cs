using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
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

        public long GetTypeCount<T>() where T : class
        {
            var status = ElasticClient.Count<T>();
            return status.Count;
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

            var descriptor = new CreateIndexDescriptor(DefaultInfrastructureName)
                .Settings(s => s
                    .NumberOfShards(1)
                    .NumberOfReplicas(0)
                )
                .Mappings(ms => ms
                    .Map<OFEmail>(m => m
                        .AutoMap()
                        .Properties(pd => pd
                            .Keyword(kd => kd.Name(e => e.ItemName).IgnoreAbove(Int32.MaxValue))
                            .Keyword(kd => kd.Name(e => e.ItemUrl).IgnoreAbove(Int32.MaxValue))
                            .Keyword(kd => kd.Name(e => e.Folder))
                            .Text(td => td.Name(e => e.Content))
                            .Text(td => td.Name(e => e.Htmlcontent))
                            .Text(td => td.Name(e => e.Analyzedcontent).Index())
                            .Keyword(kd => kd.Name(e => e.Subject).IgnoreAbove(Int32.MaxValue))
                        ))
                    .Map<OFContact>(m => m
                        .AutoMap()
                        .Properties(pd => pd
                            .Date(dd => dd
                                .Name(c => c.Birthday)
                                .Format("yyyy-MM-dd'T'HH:mm:ss.SSS")
                                .NullValue(new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, DateTime.MinValue.Hour, DateTime.MinValue.Minute, DateTime.MinValue.Second, 111))))
                    )
                    .Map<OFAttachmentContent>(m => m
                        .AutoMap()
                        .Properties(pd => pd
                            .Text(td => td.Name(a => a.Analyzedcontent).Index())
                            .Text(td => td.Name(a => a.Content))
                            .Keyword(kd => kd.Name(a => a.Filename).IgnoreAbove(Int32.MaxValue))

                        ))
                    .Map<OFStore>(m => m.AutoMap())
                    .Map<OFShortContact>(m => m.AutoMap())
                );

            var response = ElasticClient.CreateIndex(descriptor);

            OFLogger.Instance.LogDebug("Create Index...");
            OFLogger.Instance.LogDebug("Status: {0}  Success: {1}", response.ApiCall.HttpStatusCode, response.ApiCall.Success);
            OFLogger.Instance.LogDebug("Debug Info: {0}", response.DebugInformation);

            return response.ApiCall.HttpStatusCode == 200;

        }

        private void CreateRiver()
        {
            var riverMeta = new OFRiverMeta(DefaultInfrastructureName);

            OFObjectJsonSaveReadHelper.Instance.SaveElasticSearchSettings(riverMeta);

            OFLogger.Instance.LogDebug("Create Settings for service contoroller...");
        }

    }
}