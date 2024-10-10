using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Marketplace
{
    
    [JsonConverter(typeof(OrderSideConverter))]
    public enum OrderSide
    {
        unknown,
        listing,
        offer,
    }
    
    public static class OrderSideExtensions
    {
        public static string AsString(this OrderSide side)
        {
            return side.ToString();
        }
    }
    
    public class OrderSideConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderSide) || objectType == typeof(OrderSide[]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is OrderSide[] paymentMethods)
            {
                writer.WriteStartArray();
                foreach (var method in paymentMethods)
                {
                    writer.WriteValue(method.AsString());
                }
                writer.WriteEndArray();
            }
            else if (value is OrderSide paymentMethod)
            {
                writer.WriteValue(paymentMethod.AsString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(OrderSide[]))
            {
                var array = JArray.Load(reader);
                var methods = new OrderSide[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    methods[i] = Enum.Parse<OrderSide>(array[i].ToString());
                }
                return methods;
            }
            else
            {
                var method = JToken.Load(reader).ToString();
                return Enum.Parse<OrderSide>(method);
            }
        }
    }
}