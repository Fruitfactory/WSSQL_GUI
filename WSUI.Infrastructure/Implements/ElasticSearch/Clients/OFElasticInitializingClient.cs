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

        private static readonly int WARM_UP_SIZER = 1000;

        [InjectionConstructor()]
        public OFElasticInitializingClient()
        {
        }

        public async void CreateInfrastructure()
        {
            try
            {
                if ( await CreateIndex())
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

        public OFStatusResponse GetIndexingProgress()
        {
            var client = CreateClient(OFIndexNames.DefaultEmailIndexName);
            return client.LowLevel.Get<OFStatusResponse>(OFIndexNames.DefaultEmailIndexName, "status");
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

                var client = CreateClient(OFIndexNames.DefaultContactIndexName);
                var result = client.Search<OFContact>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
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
                var client = CreateClient(OFIndexNames.DefaultEmailIndexName);
                var result = client.Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                    q.Term(e => e.Subject, value)));
                result = client.Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                    q.Term(e => e.Analyzedcontent, value)));
                result = client.Search<OFEmail>(s => s.From(0).Size(WARM_UP_SIZER).Query(queryDescriptor => queryDescriptor.Bool(bd => bd.Should(
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
                var client = CreateClient(OFIndexNames.DefaultAttachmentIndexName);
                var result = client.Search<OFAttachmentContent>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
                    q.Term(a => a.Filename, image)));
                result = client.Search<OFAttachmentContent>(s => s.From(0).Size(WARM_UP_SIZER).Query(q =>
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

        private async Task<bool> CreateIndex()
        {
            var result =  await CreateIndexForEmails() && await CreateIndexForAttachmentContent() &&  await CreateIndexForContacts() &&
                   await CreateIndexForShortContact() && await CreateIndexForStore();
            OFLogger.Instance.LogDebug("Create Index...");
            return result;

        }

        private async Task<bool> CreateIndexForEmails()
        {
            var connectionSettings = GetConnectionSetting(OFIndexNames.DefaultEmailIndexName);
            connectionSettings.DefaultMappingFor<OFEmail>(s =>
                s.IdProperty(e => e.Entryid).IndexName(OFIndexNames.DefaultEmailIndexName));
            connectionSettings.DefaultMappingFor<OFRecipient>(s =>
                s.IdProperty(r => r.Entryid).IndexName(OFIndexNames.DefaultEmailIndexName));
            connectionSettings.DefaultMappingFor<OFAttachment>(s =>
                s.IdProperty(a => a.Entryid).IndexName(OFIndexNames.DefaultEmailIndexName));

            var client = new ElasticClient(connectionSettings);
            var response = await client.Indices.CreateAsync(OFIndexNames.DefaultEmailIndexName, c => c
                               .Map<OFEmail>(m => m
                                                  .AutoMap()
                                                  .Properties(p => p
                                                      .Nested<OFRecipient>(np => np
                                                                                 .AutoMap()
                                                                                 .Name(e => e.To)
                                                                                 .Name(e => e.Cc)
                                                                                 .Name(e => e.Bcc))
                                                      .Nested<OFAttachment>(np => np
                                                          .AutoMap()
                                                          .Name(e => e.Attachments))
                                                  )
                                                  )
                           );

            return response.ServerError.IsNull();
        }

        private async Task<bool> CreateIndexForContacts()
        {
            var connectionSettings = GetConnectionSetting(OFIndexNames.DefaultContactIndexName);
            connectionSettings.DefaultMappingFor<OFContact>(s => s
                                                                 .IdProperty(c => c.Entryid)
                                                                 .IndexName(OFIndexNames.DefaultContactIndexName));

            var client = new ElasticClient(connectionSettings);
            var response = await client.Indices.CreateAsync(OFIndexNames.DefaultContactIndexName,
                               d => d.Map<OFContact>(m => m.AutoMap()));

            return response.ServerError.IsNull();
        }

        private async Task<bool> CreateIndexForAttachmentContent()
        {
            var connectionSettings = GetConnectionSetting(OFIndexNames.DefaultAttachmentIndexName);
            connectionSettings.DefaultMappingFor<OFAttachmentContent>(s => s
                                                                           .IdProperty(a => a.Entryid)
                                                                           .IndexName(OFIndexNames
                                                                               .DefaultAttachmentIndexName));
            var client = new ElasticClient(connectionSettings);
            var response = await client.Indices.CreateAsync(OFIndexNames.DefaultAttachmentIndexName, d => d
                .Map<OFAttachmentContent>(m => m.AutoMap()));

            return response.ServerError.IsNull();
        }

        private async Task<bool> CreateIndexForStore()
        {
            var connectionSettings = GetConnectionSetting(OFIndexNames.DefaultStoreIndexName);
            connectionSettings.DefaultMappingFor<OFStore>(s =>
                s.IdProperty(store => store.Storeid).IndexName(OFIndexNames.DefaultStoreIndexName));

            var client = new ElasticClient(connectionSettings);
            var response = await client.Indices.CreateAsync(OFIndexNames.DefaultStoreIndexName,
                               d => d.Map<OFStore>(m => m.AutoMap()));

            return response.ServerError.IsNull();
        }

        private async Task<bool> CreateIndexForShortContact()
        {
            var connectionSettings = GetConnectionSetting(OFIndexNames.DefaultShortContactIndexName);
            connectionSettings.DefaultMappingFor<OFShortContact>(s =>
                s.IdProperty(c => c.Email).IndexName(OFIndexNames.DefaultShortContactIndexName));

            var client = new ElasticClient(connectionSettings);
            var response = await client.Indices.CreateAsync(OFIndexNames.DefaultShortContactIndexName,
                               d => d.Map<OFShortContact>(m => m.AutoMap()));

            return response.ServerError.IsNull();
        }

        private void CreateRiver()
        {
            var riverMeta = new OFRiverMeta(OFIndexNames.DefaultEmailIndexName);

            OFObjectJsonSaveReadHelper.Instance.SaveElasticSearchSettings(riverMeta);

            OFLogger.Instance.LogDebug("Create Settings for service controller...");
        }
    }
}