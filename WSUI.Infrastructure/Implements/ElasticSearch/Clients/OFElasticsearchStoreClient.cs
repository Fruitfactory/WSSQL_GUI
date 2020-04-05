using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Nest;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Interfaces;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticsearchStoreClient : OFElasticSearchClientBase, IOFElasticsearchStoreClient
    {
        [InjectionConstructor]
        public OFElasticsearchStoreClient()
        {
            
        }
        
        public bool SaveStore(OFStore Store)
        {
            var result = ElasticClient.IndexDocument(Store);
            return result.Result == Result.Created;
        }

        public bool DeleteStore(OFStore Store)
        {
            var resultEmail =
                ElasticClient.DeleteByQuery<OFEmail>(q => q.Query(d => d.Term(e => e.Storeid, Store.Storeid)));
            var resultContact =
                ElasticClient.DeleteByQuery<OFContact>(q => q.Query(d => d.Term(c => c.Storeid, Store.Storeid)));
            var resultAttach =
                ElasticClient.DeleteByQuery<OFAttachmentContent>(
                    q => q.Query(d => d.Term(a => a.Storeid, Store.Storeid)));

            var result = ElasticClient.DeleteByQuery<OFStore>(q => q.Query(d => d.Term(s => s.Storeid, Store.Storeid)));
            return resultEmail.IsValid && resultContact.IsValid && resultAttach.IsValid && result.IsValid;
        }

        public IEnumerable<OFStore> GetStores()
        {
            var result = ElasticClient.Search<OFStore>(qd => qd.From(0).Size(100));
            return result.Documents;
        }
        
    }
}