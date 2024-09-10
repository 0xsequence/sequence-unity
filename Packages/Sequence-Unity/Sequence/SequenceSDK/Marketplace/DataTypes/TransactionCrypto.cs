using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(TransactionCryptoConverter))]
    public enum TransactionCrypto
    {
        none,
        partially,
        all
    }
    
    public class TransactionCryptoConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TransactionCrypto) || objectType == typeof(TransactionCrypto[]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is TransactionCrypto[] paymentMethods)
            {
                writer.WriteStartArray();
                foreach (var method in paymentMethods)
                {
                    writer.WriteValue(method.ToString());
                }
                writer.WriteEndArray();
            }
            else if (value is TransactionCrypto paymentMethod)
            {
                writer.WriteValue(paymentMethod.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(TransactionCrypto[]))
            {
                var array = JArray.Load(reader);
                var methods = new TransactionCrypto[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    methods[i] = Enum.Parse<TransactionCrypto>(array[i].ToString());
                }
                return methods;
            }
            else
            {
                var method = JToken.Load(reader).ToString();
                return Enum.Parse<TransactionCrypto>(method);
            }
        }
    }
}