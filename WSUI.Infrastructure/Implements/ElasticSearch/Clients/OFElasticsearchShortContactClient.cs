using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Nest;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticsearchShortContactClient : OFElasticSearchClientBase, IOFElasticsearchShortContactClient
    {

        [InjectionConstructor]
        public OFElasticsearchShortContactClient()
        {
            
        }

        public void SaveShortContacts(List<OFShortContact> contacts)
        {
            if (contacts.IsNull() || !contacts.Any())
            {
                return;
            }

            var split = contacts.Split(50);

            foreach (var list in split)
            {
                var descriptor = new BulkDescriptor();
                foreach (var item in list)
                {
                    descriptor.Index<OFShortContact>(op => op.Document(item));
                }
                var response = ElasticClient.Bulk(descriptor);
                OFLogger.Instance.LogDebug($"Bulk response: {response.IsValid}");
            }
        }
        
        public IEnumerable<OFShortContact> GetAllSuggestionContacts()
        {
            var countResponce = ElasticClient.Count<OFShortContact>();
            var count = (int)(countResponce.IsNotNull() ? countResponce.Count : 10000);
            
            var result = ElasticClient.Search<OFShortContact>(qd => qd.From(0).Size(count).MatchAll());
            return result.Documents;
        }
    }
}