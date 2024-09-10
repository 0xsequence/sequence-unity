using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(MarketplaceKindConverter))]
    public enum MarketplaceKind
    {
        unknown,
        sequence_marketplace_v1,
        sequence_marketplace_v2,
        opensea,
        magic_eden,
        mintify,
        looks_rare,
        x2y2,
        sudo_swap,
        coinbase,
        rarible,
        nftx,
        foundation,
        manifold,
        zora,
        blur, 
        super_rare,
        okx,
        element,
        aqua_xyz,
        auranft_co,
    }

    public static class MarketplaceKindExtensions
    {
        public static string AsString(this MarketplaceKind kind)
        {
            return kind.ToString();
        }
    }
    
    public class MarketplaceKindConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MarketplaceKind) || objectType == typeof(MarketplaceKind[]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is MarketplaceKind[] paymentMethods)
            {
                writer.WriteStartArray();
                foreach (var method in paymentMethods)
                {
                    writer.WriteValue(method.AsString());
                }
                writer.WriteEndArray();
            }
            else if (value is MarketplaceKind paymentMethod)
            {
                writer.WriteValue(paymentMethod.AsString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(MarketplaceKind[]))
            {
                var array = JArray.Load(reader);
                var methods = new MarketplaceKind[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    methods[i] = Enum.Parse<MarketplaceKind>(array[i].ToString());
                }
                return methods;
            }
            else
            {
                var method = JToken.Load(reader).ToString();
                return Enum.Parse<MarketplaceKind>(method);
            }
        }
    }
}