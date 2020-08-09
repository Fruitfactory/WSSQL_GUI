using System;
using System.Text;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Converter;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.JsonSettings;
using OF.Core.Logger;

namespace OF.Core.Core.ElasticSearch
{
    public abstract class OFElasticSearchClientInstanceBase<E> : OFElasticSearchClientBase where E : class, IElasticSearchObject, new()
    {
        private OFJsonSettings _settings;

        protected OFElasticSearchClientInstanceBase()
        {
            Init();
        }

        private void Init()
        {
            try
            {
                string defaultIndexName = GetIndexName(typeof(E));
                ElasticClient = CreateClient(defaultIndexName);
                _settings = new OFJsonSettings();
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

        protected virtual string GetDefaultIndexName()
        {
            return OFIndexNames.DefaultEmailIndexName;
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