using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(TransactionProviderConverter))]
    public enum TransactionProvider
    {
        unknown,
        sardine,
        transak,
        zerox
    }
    
    public class TransactionProviderConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TransactionProvider) || objectType == typeof(TransactionProvider[]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is TransactionProvider[] paymentMethods)
            {
                writer.WriteStartArray();
                foreach (var method in paymentMethods)
                {
                    writer.WriteValue(method.ToString());
                }
                writer.WriteEndArray();
            }
            else if (value is TransactionProvider paymentMethod)
            {
                writer.WriteValue(paymentMethod.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(TransactionProvider[]))
            {
                var array = JArray.Load(reader);
                var methods = new TransactionProvider[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    methods[i] = Enum.Parse<TransactionProvider>(array[i].ToString());
                }
                return methods;
            }
            else
            {
                var method = JToken.Load(reader).ToString();
                return Enum.Parse<TransactionProvider>(method);
            }
        }
    }
}