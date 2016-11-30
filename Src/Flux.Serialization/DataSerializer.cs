using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Flux.Serialization
{
    /// <summary>Contains methods for data serialization/deserialization.</summary>
    public static class DataSerializer
    {
        /// <summary>Deserializes data using Newtonsoft.Json library to class T.</summary>
        /// <typeparam name="T">Result data type.</typeparam>
        /// <param name="data">Json string to deserialize.</param>
        /// <returns>Deserialized object.</returns>
        public static T Deserialize<T>(string data)
        {
            using (var reader = new StringReader(data))
            {
                return Deserialize<T>(reader);
            }
        }

        /// <summary>Deserializes stream using Newtonsoft.Json library to class T.</summary>
        /// <typeparam name="T">Result data type.</typeparam>
        /// <param name="jsonStream">Json stream to deserialize.</param>
        /// <returns>Deserialized object.</returns>
        public static T Deserialize<T>(Stream jsonStream)
        {
            using (StreamReader reader = new StreamReader(jsonStream))
            {
                return Deserialize<T>(reader);
            }
        }

        /// <summary>Deserializes data using Newtonsoft.Json library to dynamic object.</summary>
        /// <param name="data">Json string to deserialize.</param>
        /// <returns>Dynamic object represents Json.</returns>
        public static dynamic DynamicDeserialize(string data)
        {
            return JObject.Parse(data);
        }

        /// <summary> Deserializes stream using Newtonsoft.Json library to dynamic object.</summary>
        /// <param name="jsonStream">Json string to deserialize.</param>
        /// <returns>Dynamic object represents Json.</returns>
        public static dynamic DynamicDeserialize(Stream jsonStream)
        {
            using (var reader = new StreamReader(jsonStream))
            {
                string data = reader.ReadToEnd();
                return DynamicDeserialize(data);
            }
        }

        /// <summary>Serializes data using Newtonsoft.Json library.</summary>
        /// <param name="data">Data to serialize.</param>
        /// <returns>Json strings with serialized data.</returns>
        public static string Serialize(object data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        private static T Deserialize<T>(TextReader reader)
        {
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTimeOffset;
            jsonSettings.Converters = new Newtonsoft.Json.JsonConverter[] { new ObjectToDictConvertor(), new VersionConverter() };
            var deserializer = Newtonsoft.Json.JsonSerializer.CreateDefault(jsonSettings);
            return (T)deserializer.Deserialize(reader, typeof(T));
        }
    }
}
