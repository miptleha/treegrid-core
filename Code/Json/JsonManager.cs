using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TreeGridCore.Code.Json
{
    public class JsonManager
    {
        public static object ReadJson(string path)
        {
            if (!File.Exists(path))
                return null;

            object res = null;
            using (var sw = new StreamReader(path))
            {
                var json = sw.ReadToEnd();
                res = ReadJson(json);
            }
            return res;
        }

        public class ObjectToInferredTypesConverter : JsonConverter<object>
        {
            public override object Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) => reader.TokenType switch
                {
                    JsonTokenType.True => true,
                    JsonTokenType.False => false,
                    JsonTokenType.Number when reader.TryGetInt64(out long l) => l,
                    JsonTokenType.Number => reader.GetDouble(),
                    JsonTokenType.String when reader.TryGetDateTime(out DateTime datetime) => datetime,
                    JsonTokenType.String => reader.GetString(),
                    _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
                };

            public override void Write(
                Utf8JsonWriter writer,
                object objectToWrite,
                JsonSerializerOptions options) =>
                JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
        }

        public static List<Dictionary<string, object>> ReadJsonString(string json)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ObjectToInferredTypesConverter());

            return JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json, options);
        }
    }
}
