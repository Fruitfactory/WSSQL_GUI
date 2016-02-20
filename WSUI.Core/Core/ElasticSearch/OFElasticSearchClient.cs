using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Elasticsearch.Net.Serialization;
using Microsoft.Practices.Unity;
using Nest;
using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Converter;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;
using Exception = System.Exception;

namespace OF.Core.Core.ElasticSearch
{
    public abstract class OFElasticSearchClientBase : IDisposable
    {
        
        public static readonly string ElasticSearchHost = "http://127.0.0.1:9200";
        public static readonly string DefaultInfrastructureName = "outlookfinder";

        private ElasticClient _internalElasticClient;
        private JsonSerializerSettings _settings;

        [InjectionConstructor]
        protected OFElasticSearchClientBase()
            : this(ElasticSearchHost)
        {
        }

        protected OFElasticSearchClientBase(string host)
            : this(host, DefaultInfrastructureName)
        {
        }

        protected OFElasticSearchClientBase(string host, string defaultIndexName)
        {
            Init(host, defaultIndexName);
        }

        private void Init(string host, string defaultIndexName)
        {
            try
            {
                var node = new Uri(host);
                var settings = new ConnectionSettings(node, defaultIndexName);
                settings.SetJsonSerializerSettingsModifier(jsonSettings =>
                {
                    jsonSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fff";
                });
                ElasticClient = new ElasticClient(settings);
                _settings = new JsonSerializerSettings();
                _settings.Converters.Add(new OFConditionCollectionConverter());
                _settings.Formatting = Formatting.Indented;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        public ElasticClient ElasticClient
        {
            get { return _internalElasticClient; }
            set { _internalElasticClient = value; }
        }

        protected INestSerializer Serializer
        {
            get
            {
                return ElasticClient.Serializer;
            }
        }

        protected IElasticsearchClient Raw
        {
            get { return ElasticClient.Raw; }
        }

        public IExistsResponse IndexExists(string name)
        {
            return ElasticClient.IndexExists(name);
        }

        public IExistsResponse IndexExists(IIndexExistsRequest indexExists)
        {
            return ElasticClient.IndexExists(indexExists);
        }


        public void Dispose()
        {
            if (ElasticClient != null)
            {
                ElasticClient = null;
            }
            GC.SuppressFinalize(this);
        }

        protected byte[] Serialize(object obj)
        {
            if (obj.IsNull())
            {
                return null;
            }

            var strResult = JsonConvert.SerializeObject(obj, _settings);
            if (strResult.IsEmpty())
            {
                return null;
            }
            return Encoding.UTF8.GetBytes(strResult);
        }


    }
}