using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Marketplace
{
    
    [JsonConverter(typeof(OrderbookKindConverter))]
    public enum OrderbookKind
    {
        unknown,
        sequence_marketplace_v1,
        sequence_marketplace_v2,
        blur,
        opensea,
        looks_rare,
        reservoir,
        x2y2,
    }
    
    public static class OrderbookKindExtensions
    {
        public static string AsString(this OrderbookKind kind)
        {
            return kind.ToString();
        }
    }
    
    internal class OrderbookKindConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderbookKind) || objectType == typeof(OrderbookKind[]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is OrderbookKind[] paymentMethods)
            {
                writer.WriteStartArray();
                foreach (var method in paymentMethods)
                {
                    writer.WriteValue(method.AsString());
                }
                writer.WriteEndArray();
            }
            else if (value is OrderbookKind paymentMethod)
            {
                writer.WriteValue(paymentMethod.AsString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(OrderbookKind[]))
            {
                var array = JArray.Load(reader);
                var methods = new OrderbookKind[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    methods[i] = Enum.Parse<OrderbookKind>(array[i].ToString());
                }
                return methods;
            }
            else
            {
                var method = JToken.Load(reader).ToString();
                return Enum.Parse<OrderbookKind>(method);
            }
        }
    }
}