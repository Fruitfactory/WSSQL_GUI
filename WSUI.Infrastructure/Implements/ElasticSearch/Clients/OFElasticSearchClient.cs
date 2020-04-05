using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Elasticsearch.Net;
using Microsoft.Practices.Unity;
using Nest;
using Newtonsoft.Json;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticSearchClient : OFElasticSearchClientBase,IElasticSearchClient
    {
        [InjectionConstructor]
        public OFElasticSearchClient()
        {
            
        }

        public ISearchResponse<T> Search<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searchSelector)
            where T : class
        {
            return ElasticClient.Search<T>(searchSelector);
        }

        public IRawSearchResult<T> RawSearch<T>(object body) where T : class, new()
        {

            byte[] bodyBytes = Serialize(body);
            IEnumerable<T> listResult = null;
            int took = 0;
            int total = 0;
            try
            {
#if DEBUG
                var str = Encoding.Default.GetString(bodyBytes);
#endif

	            var postData = PostData.Bytes(bodyBytes);
                //var result = Raw.Search<byte[]>(DefaultInfrastructureName, GetSearchType(typeof(T)), bodyBytes);
                var result = Raw.Search<OFResponseRaw<T>>(DefaultInfrastructureName,postData);
                //using (var stream = new MemoryStream(result.ResponseBodyInBytes))
                //using (var reader = new StreamReader(stream))
                //{
                //    var rawResult = JsonSerializer.Create().Deserialize(reader, typeof(OFResponseRaw<T>)) as OFResponseRaw<T>;
                took = (int)result.took;
                total = result.hits.total;
                listResult = result.hits.hits.Select(h => h._source);
                //}
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }

            return new OFRawSearchResponse<T>(took, total, listResult);
        }

        private string GetSearchType(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(ElasticsearchTypeAttribute), false);
            string result = "";
            if (attrs != null && attrs.Length > 0)
            {
                var elasticType = attrs[0] as ElasticsearchTypeAttribute;
                result = elasticType.Name;
            }
            return result;
        }


    }
}