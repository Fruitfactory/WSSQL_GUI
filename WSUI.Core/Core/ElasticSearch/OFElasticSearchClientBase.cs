using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Practices.Unity;
using Nest;
using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Converter;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.JsonSettings;
using OF.Core.Logger;
using Exception = System.Exception;

namespace OF.Core.Core.ElasticSearch
{
    public abstract class OFElasticSearchClientBase : IDisposable
    {
        
        public static readonly string ElasticSearchHost = "http://127.0.0.1:9200";

        protected OFElasticSearchClientBase()
        {
        }

        public ExistsResponse IndexExists(string name)
        {
            var client = CreateClient(name);
            return client.Indices.Exists(Indices.Index(name));
        }

        public long GetTypeCount<T>() where T : OFElasticSearchBaseEntity
        {
            var type = typeof(T).Name;
            var defaultIdex = OFIndexNames.DefaultEmailIndexName;
            switch (type)
            {
                case "OFAttachmentContent":
                    defaultIdex = OFIndexNames.DefaultAttachmentIndexName;
                    break;
                case "OFContact":
                    defaultIdex = OFIndexNames.DefaultContactIndexName;
                    break;
                case "OFShortContact":
                    defaultIdex = OFIndexNames.DefaultShortContactIndexName;
                    break;
                case "OFStore":
                    defaultIdex = OFIndexNames.DefaultStoreIndexName;
                    break;
                case "OFEmail":
                default:
                    defaultIdex = OFIndexNames.DefaultEmailIndexName;
                    break;
            }

            var client = CreateClient(defaultIdex);
            var status = client.Count<T>();
            return status.Count;
        }


        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected ConnectionSettings GetConnectionSetting(string defaultIndex)
        {
            var cs = new ConnectionSettings(new Uri(ElasticSearchHost));
            cs.DefaultIndex(defaultIndex);
            return cs;
        }

        protected ElasticClient CreateClient(string indexName)
        {
            var setting = GetConnectionSetting(indexName);
            return new ElasticClient(setting);
        }

    }
}