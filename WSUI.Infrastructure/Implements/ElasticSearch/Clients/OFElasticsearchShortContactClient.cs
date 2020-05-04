using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Nest;
using OF.Core;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticsearchShortContactClient : OFElasticSearchClientInstanceBase, IOFElasticsearchShortContactClient
    {

        [InjectionConstructor]
        public OFElasticsearchShortContactClient()
        {
            
        }

        protected override string GetDefaultIndexName()
        {
            return OFIndexNames.DefaultShortContactIndexName;
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
            var scanResult = ElasticClient.Search<OFShortContact>(qd => qd.From(0).Size(GlobalConst.DefaultCountOfContacts).MatchAll().Scroll(GlobalConst.DefaultScrollScanTime));
            var documents= new List<OFShortContact>();

            var result = ElasticClient.Scroll<OFShortContact>(GlobalConst.DefaultScrollingTime, scanResult.ScrollId);

            while (result.Documents.Any())
            {
                documents.AddRange(result.Documents);
                result = ElasticClient.Scroll<OFShortContact>(GlobalConst.DefaultScrollingTime, result.ScrollId);
            }

            return documents;
        }
    }
}