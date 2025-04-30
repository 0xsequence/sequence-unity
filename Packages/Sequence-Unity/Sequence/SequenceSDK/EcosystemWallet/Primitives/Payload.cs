using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    internal abstract class Payload
    {
        public abstract PayloadType type { get; }
        [CanBeNull] public Address[] parentWallets { get; set; }

        public bool isCalls => type == PayloadType.Call;
        public bool isMessage => type == PayloadType.Message;
        public bool isConfigUpdate => type == PayloadType.ConfigUpdate;
        public bool isDigest => type == PayloadType.Digest;
    }
    
    [JsonConverter(typeof(PayloadTypeConverter))] // We use a custom converter here instead of EnumConverter because ConfigUpdate -> config-update and '-' aren't supported in enums in C#
    internal enum PayloadType 
    {
        Call,
        Message,
        ConfigUpdate, 
        Digest,
    }
    
    internal class PayloadTypeConverter : JsonConverter<PayloadType>
    {
        public override void WriteJson(JsonWriter writer, PayloadType value, JsonSerializer serializer)
        {
            var stringValue = value switch
            {
                PayloadType.ConfigUpdate => "config-update",
                _ => value.ToString().ToLowerInvariant()
            };

            writer.WriteValue(stringValue);
        }

        public override PayloadType ReadJson(JsonReader reader, Type objectType, PayloadType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string? stringValue = (reader.Value ?? JToken.Load(reader).ToString())?.ToString().ToLowerInvariant();

            return stringValue switch
            {
                "call" => PayloadType.Call,
                "message" => PayloadType.Message,
                "config-update" => PayloadType.ConfigUpdate,
                "digest" => PayloadType.Digest,
                _ => throw new JsonSerializationException($"Unknown PayloadType value: {stringValue}")
            };
        }
    }
}