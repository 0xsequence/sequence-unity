using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.EcosystemWallet.Primitives.Common
{
    [JsonConverter(typeof(BytesConverter))]
    public class Bytes
    {
        public byte[] Data { get; set; }

        public Bytes(byte[] data)
        {
            Data = data;
        }

        public static implicit operator byte[](Bytes b) => b.Data;
        public static implicit operator Bytes(byte[] data) => new Bytes(data);
    }

    public class BytesConverter : JsonConverter<Bytes>
    {
        public override void WriteJson(JsonWriter writer, Bytes value, JsonSerializer serializer)
        {
            var hex = "0x" + BitConverter.ToString(value.Data).Replace("-", "").ToLowerInvariant();
            writer.WriteStartObject();
            writer.WritePropertyName("_isUint8Array");
            writer.WriteValue(true);
            writer.WritePropertyName("data");
            writer.WriteValue(hex);
            writer.WriteEndObject();
        }

        public override Bytes ReadJson(JsonReader reader, Type objectType, Bytes existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var hexString = obj["data"]?.Value<string>() ?? "0x";

            if (!hexString.StartsWith("0x"))
                throw new JsonSerializationException("Expected hex string to start with '0x'.");

            var hex = hexString.Substring(2);
            if (hex.Length % 2 != 0)
                throw new JsonSerializationException("Hex string must have an even length.");

            var byteArray = new byte[hex.Length / 2];
            for (int i = 0; i < byteArray.Length; i++)
                byteArray[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

            return new Bytes(byteArray);
        }
    }
}