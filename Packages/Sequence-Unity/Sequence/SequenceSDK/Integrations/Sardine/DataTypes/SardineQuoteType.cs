using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Integrations.Sardine
{
    [JsonConverter(typeof(EnumConverter<SardineQuoteType>))]
    internal enum SardineQuoteType
    {
        buy,
        sell
    }
}