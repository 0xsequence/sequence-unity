using System;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives.Common
{
    [Preserve]
    [JsonConverter(typeof(BigIntConverter))]
    public class BigInt
    {
        public BigInteger Value { get; set; }

        public BigInt(BigInteger value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator BigInteger(BigInt b) => b.Value;
        public static implicit operator BigInt(BigInteger value) => new BigInt(value);
    }

    [Preserve]
    public class BigIntConverter : JsonConverter<BigInt>
    {
        public override void WriteJson(JsonWriter writer, BigInt value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("_isBigInt");
            writer.WriteValue(true);
            writer.WritePropertyName("data");
            writer.WriteValue(value.Value.ToString()); // decimal string
            writer.WriteEndObject();
        }

        public override BigInt ReadJson(JsonReader reader, Type objectType, BigInt existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            if (obj["_isBigInt"]?.Value<bool>() != true)
                throw new JsonSerializationException("Expected _isBigInt to be true.");

            var dataStr = obj["data"]?.Value<string>();
            if (string.IsNullOrEmpty(dataStr))
                throw new JsonSerializationException("Missing or empty data field for BigInt.");

            if (!BigInteger.TryParse(dataStr, out var value))
                throw new JsonSerializationException("Invalid BigInt format.");

            return new BigInt(value);
        }
    }
}