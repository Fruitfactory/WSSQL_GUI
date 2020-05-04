using System;
using System.Text;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Converter;
using OF.Core.Extensions;
using OF.Core.JsonSettings;
using OF.Core.Logger;

namespace OF.Core.Core.ElasticSearch
{
    public abstract class OFElasticSearchClientInstanceBase : OFElasticSearchClientBase
    {
        private OFJsonSettings _settings;

        protected OFElasticSearchClientInstanceBase()
        {
        }

        protected OFElasticSearchClientInstanceBase(string host)
        {
            Init(host);
        }

        private void Init(string host)
        {
            try
            {
                string defaultIndexName = GetDefaultIndexName();
                var node = new Uri(host);
                var settings = new ConnectionSettings(node).DefaultIndex(defaultIndexName).DisableDirectStreaming();
                ElasticClient = new ElasticClient(settings);
                _settings = new OFJsonSettings();
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        protected abstract string GetDefaultIndexName();

        public ElasticClient ElasticClient { get; private set; }

        //TODO: check carefully
        protected IElasticsearchSerializer Serializer => ElasticClient.SourceSerializer;

        protected IElasticLowLevelClient Raw => ElasticClient.LowLevel;

        protected byte[] Serialize(object obj)
        {
            if (obj.IsNull())
            {
                return null;
            }

            var strResult = JsonConvert.SerializeObject(obj, _settings.Settings);
            if (strResult.IsEmpty())
            {
                return null;
            }

            return Encoding.UTF8.GetBytes(strResult);
        }

        protected JsonSerializerSettings GetJsonSettings()
        {
            return _settings.Settings;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (ElasticClient != null)
            {
                ElasticClient = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}