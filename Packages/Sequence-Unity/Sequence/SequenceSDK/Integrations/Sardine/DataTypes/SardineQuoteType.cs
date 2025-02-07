using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Integrations.Sardine
{
    [JsonConverter(typeof(EnumConverter<SardineQuoteType>))]
    public enum SardineQuoteType
    {
        buy,
        sell
    }
}