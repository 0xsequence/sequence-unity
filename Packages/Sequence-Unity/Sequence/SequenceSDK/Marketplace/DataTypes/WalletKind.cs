using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.EmbeddedWallet;

namespace Sequence.Marketplace
{
    
    [JsonConverter(typeof(WalletKindConverter))]
    public enum WalletKind
    {
        unknown,
        sequence
    }
    
    public static class WalletKindExtensions {
        public static WalletKind GetWalletKind(this IWallet wallet)
        {
            if (wallet is SequenceWallet)
            {
                return WalletKind.sequence;
            }

            return WalletKind.unknown;
        }
        
        public static string AsString(this WalletKind kind)
        {
            return kind.ToString();
        }
    }
    
    public class WalletKindConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(WalletKind) || objectType == typeof(WalletKind[]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is WalletKind[] walletKinds)
            {
                writer.WriteStartArray();
                foreach (var method in walletKinds)
                {
                    writer.WriteValue(method.AsString());
                }
                writer.WriteEndArray();
            }
            else if (value is WalletKind wallet)
            {
                writer.WriteValue(wallet.AsString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(WalletKind[]))
            {
                var array = JArray.Load(reader);
                var methods = new WalletKind[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    methods[i] = Enum.Parse<WalletKind>(array[i].ToString());
                }
                return methods;
            }
            else
            {
                var method = JToken.Load(reader).ToString();
                return Enum.Parse<WalletKind>(method);
            }
        }
    }
}