using Microsoft.Practices.Unity;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticSearchRemovingClient : OFElasticSearchClientInstanceBase<OFEmail>,IOFElasticSearchRemovingClient
    {
        [InjectionConstructor]
        public OFElasticSearchRemovingClient()
        {
            
        }

        public void RemoveEmail(string entryId)
        {
            var result = ElasticClient.DeleteByQuery<OFEmail>(q => q.Query(rq => rq.Term(e => e.Entryid, entryId)));
            OFLogger.Instance.LogInfo("Remove Email: {0}",result.ApiCall.Success);
        }

    }
}