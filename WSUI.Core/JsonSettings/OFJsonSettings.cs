using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Converter;

namespace OF.Core.JsonSettings
{
    public class OFJsonSettings
    {
        private JsonSerializerSettings _settings;


        public OFJsonSettings()
        {
            _settings = new JsonSerializerSettings();
            _settings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fff";
            _settings.ContractResolver = new OFLowercaseContractResolver();
            _settings.Converters.Add(new OFConditionCollectionConverter());
            _settings.Formatting = Formatting.Indented;
        }

        public JsonSerializerSettings Settings => _settings;

    }
}