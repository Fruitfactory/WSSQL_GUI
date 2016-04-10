using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
                Stopwatch watch = new Stopwatch();
                watch.Start();

                var str = Encoding.UTF8.GetString(bodyBytes);

                var result = Raw.Search<byte[]>(DefaultInfrastructureName, GetSearchType(typeof(T)), bodyBytes);

                watch.Stop();
                OFLogger.Instance.LogDebug("Search (Send request to ES and retrieve response): {0}ms", watch.ElapsedMilliseconds);

                Stopwatch watchParsing = new Stopwatch();
                watchParsing.Start();

                //str = Encoding.UTF8.GetString(result.Response);

                using (var stream = new MemoryStream(result.Response))
                using (var reader = new StreamReader(stream))
                {
                    var rawResult = JsonSerializer.Create().Deserialize(reader, typeof(OFResponseRaw<T>)) as OFResponseRaw<T>;
                    took = (int)rawResult.took;
                    total = rawResult.hits.total;
                    listResult = rawResult.hits.hits.Select(h => h._source);
                }
                watchParsing.Stop();
                OFLogger.Instance.LogDebug("Parsing dynamic response: {0}ms, Diff: {1}ms", watchParsing.ElapsedMilliseconds, Math.Abs(watchParsing.ElapsedMilliseconds - watch.ElapsedMilliseconds));

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }

            return new OFRawSearchResponse<T>(took, total, listResult);
        }

        private string GetSearchType(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(ElasticTypeAttribute), false);
            string result = "";
            if (attrs != null && attrs.Length > 0)
            {
                var elasticType = attrs[0] as ElasticTypeAttribute;
                result = elasticType.Name;
            }
            return result;
        }


    }
}