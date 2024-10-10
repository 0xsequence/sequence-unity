using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    
    [JsonConverter(typeof(EnumConverter<OrderbookKind>))]
    public enum OrderbookKind
    {
        unknown,
        sequence_marketplace_v1,
        sequence_marketplace_v2,
        blur,
        opensea,
        looks_rare,
        reservoir,
        x2y2,
    }
}