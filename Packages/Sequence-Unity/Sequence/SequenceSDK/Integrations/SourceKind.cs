using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(SourceKindConverter))]
    public enum SourceKind
    {
        unknown,
        external,
        sequence_marketplace_v1,
        sequence_marketplace_v2,
    }
    
    public static class SourceKindExtensions
    {
        public static string AsString(this SourceKind source)
        {
            return source.ToString();
        }
    }
    
    public class SourceKindConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SourceKind) || objectType == typeof(SourceKind[]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is SourceKind[] paymentMethods)
            {
                writer.WriteStartArray();
                foreach (var method in paymentMethods)
                {
                    writer.WriteValue(method.AsString());
                }
                writer.WriteEndArray();
            }
            else if (value is SourceKind paymentMethod)
            {
                writer.WriteValue(paymentMethod.AsString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(SourceKind[]))
            {
                var array = JArray.Load(reader);
                var methods = new SourceKind[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    methods[i] = Enum.Parse<SourceKind>(array[i].ToString());
                }
                return methods;
            }
            else
            {
                var method = JToken.Load(reader).ToString();
                return Enum.Parse<SourceKind>(method);
            }
        }
    }
}