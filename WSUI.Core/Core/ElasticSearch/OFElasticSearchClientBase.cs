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
        public static readonly string DefaultInfrastructureName = "outlookfinder";

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
                var settings = new ConnectionSettings(node).DefaultIndex(DefaultInfrastructureName).DisableDirectStreaming();

                //settings.SetJsonSerializerSettingsModifier(jsonSettings =>
                //{
                //    jsonSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fff";
                //});
                
                ElasticClient = new ElasticClient(settings);
                _settings = new JsonSerializerSettings();
                _settings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fff";
                _settings.ContractResolver = new OFLowercaseContractResolver();
                _settings.Converters.Add(new OFConditionCollectionConverter());
                _settings.Formatting = Formatting.Indented;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        public ElasticClient ElasticClient { get; private set; }

        //TODO: check carefully
        protected IElasticsearchSerializer Serializer => ElasticClient.SourceSerializer;

        protected IElasticLowLevelClient Raw => ElasticClient.LowLevel;

        public ExistsResponse IndexExists(string name)
        {
	        return ElasticClient.Indices.Exists(Indices.Index(name));
        }

        //public ExistsResponse IndexExists(IIndexExistsRequest indexExists)
        //{
        //    return ElasticClient.IndexExists(indexExists);
        //}


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

        protected JsonSerializerSettings GetJsonSettings()
        {
            return _settings;
        }


    }
}