using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<OrderSide>))]
    public enum OrderSide
    {
        unknown,
        listing,
        offer,
    }
}