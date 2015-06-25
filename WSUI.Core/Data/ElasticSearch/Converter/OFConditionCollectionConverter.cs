using System;
using Nest;
using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch.Request;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Converter
{
    public class OFConditionCollectionConverter  : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            foreach (var condition in (OFConditionCollection)value)
            {
                writer.WritePropertyName(GetConditionKey(condition));
                serializer.Serialize(writer,condition.Value);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private string GetConditionKey(OFBaseCondition<object> condition)
        {
            return condition.Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (OFConditionCollection);
        }
    }
}