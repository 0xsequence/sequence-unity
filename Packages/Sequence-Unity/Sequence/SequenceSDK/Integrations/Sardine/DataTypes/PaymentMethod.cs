using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Integrations.Sardine
{
    [JsonConverter(typeof(PaymentMethodConverter))]
    public enum PaymentMethod
    {
        us_debit,
        us_credit,
        international_debit,
        international_credit,
        ach
    }

    public static class PaymentMethodsExtensions
    {
        public static string AsString(this PaymentMethod paymentMethod)
        {
            return paymentMethod.ToString();
        }
    }
    
    public class PaymentMethodConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PaymentMethod) || objectType == typeof(PaymentMethod[]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is PaymentMethod[] paymentMethods)
            {
                writer.WriteStartArray();
                foreach (var method in paymentMethods)
                {
                    writer.WriteValue(method.AsString());
                }
                writer.WriteEndArray();
            }
            else if (value is PaymentMethod paymentMethod)
            {
                writer.WriteValue(paymentMethod.AsString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(PaymentMethod[]))
            {
                var array = JArray.Load(reader);
                var methods = new PaymentMethod[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    methods[i] = Enum.Parse<PaymentMethod>(array[i].ToString());
                }
                return methods;
            }
            else
            {
                var method = JToken.Load(reader).ToString();
                return Enum.Parse<PaymentMethod>(method);
            }
        }
    }
}