using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Flux.Serialization
{
    internal class ObjectToDictConvertor : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object) || typeof(IDictionary<string, object>).IsAssignableFrom(objectType);
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                reader.Read();

                var values = new List<object>();

                while (reader.TokenType != JsonToken.EndArray)
                {
                    values.Add(ReadJson(reader, typeof(object), existingValue, serializer));
                    reader.Read();
                }

                return values.ToArray();
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                reader.Read();

                var values = new Dictionary<string, object>();

                while (reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                        throw new ArgumentException("Property name should be string");

                    string propertyName = reader.Value.ToString();
                    reader.Read();

                    values.Add(propertyName, ReadJson(reader, typeof(object), existingValue, serializer));
                    reader.Read();
                }
                return values;
            }

            return serializer.Deserialize(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}