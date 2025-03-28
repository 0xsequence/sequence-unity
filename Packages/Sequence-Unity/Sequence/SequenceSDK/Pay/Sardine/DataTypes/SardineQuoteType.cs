using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Pay.Sardine
{
    [JsonConverter(typeof(EnumConverter<SardineQuoteType>))]
    public enum SardineQuoteType
    {
        buy,
        sell
    }
}