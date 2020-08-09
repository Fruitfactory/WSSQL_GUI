using System.Collections.Generic;
using System.Net.Sockets;
using Microsoft.Practices.Unity;
using Nest;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Interfaces;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticsearchStoreClient : OFElasticSearchClientInstanceBase<OFStore>, IOFElasticsearchStoreClient
    {
        [InjectionConstructor]
        public OFElasticsearchStoreClient()
        {
            
        }

        protected override string GetDefaultIndexName()
        {
            return OFIndexNames.DefaultStoreIndexName;
        }

        public bool SaveStore(OFStore Store)
        {
            var result = ElasticClient.IndexDocument(Store);
            return result.Result == Result.Created;
        }

        public bool DeleteStore(OFStore Store)
        {
            var resultEmail = DeleteEmails(Store.Storeid);
            var resultContact = DeleteContacts(Store.Storeid);
            var resultAttach = DeleteAttachments(Store.Storeid);
            var result = ElasticClient.DeleteByQuery<OFStore>(q => q.Query(d => d.Term(s => s.Storeid, Store.Storeid)));

            return resultEmail.IsValid && resultContact.IsValid && resultAttach.IsValid && result.IsValid;
        }

        public IEnumerable<OFStore> GetStores()
        {
            var result = ElasticClient.Search<OFStore>(qd => qd.From(0).Size(100));
            return result.Documents;
        }

        private DeleteByQueryResponse DeleteEmails(string storeId)
        {
            var client = CreateClient(OFIndexNames.DefaultEmailIndexName);
            var resultEmail =
                client.DeleteByQuery<OFEmail>(q => q.Query(d => d.Term(e => e.Storeid, storeId)));
            return resultEmail;
        }

        private DeleteByQueryResponse DeleteContacts(string storeId)
        {
            var client = CreateClient(OFIndexNames.DefaultContactIndexName);
            var resultContact =
                client.DeleteByQuery<OFContact>(q => q.Query(d => d.Term(c => c.Storeid, storeId)));
            return resultContact;
        }

        private DeleteByQueryResponse DeleteAttachments(string storeId)
        {

            var client = CreateClient(OFIndexNames.DefaultAttachmentIndexName);
            var resultAttach =
                client.DeleteByQuery<OFAttachmentContent>(
                    q => q.Query(d => d.Term(a => a.Storeid, storeId)));
            return resultAttach;
        }


        
    }
}