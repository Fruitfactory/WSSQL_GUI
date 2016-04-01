using Microsoft.Practices.Unity;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Interfaces;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticSearchRemovingClient : OFElasticSearchClientBase,IOFElasticSearchRemovingClient
    {
        [InjectionConstructor]
        public OFElasticSearchRemovingClient()
        {
            
        }
        
        public void RemoveEmail(string entryId)
        {
            var result = ElasticClient.DeleteByQuery<OFEmail>(q => q.Query(rq => rq.Term(e => e.Entryid, entryId)));
            System.Diagnostics.Debug.WriteLine(result.Found);            
        }

    }
}