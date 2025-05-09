using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<CurrencyStatus>))]
    public enum CurrencyStatus
    {
        unknown,
        created,
        syncing_metadata,
        active,
        failed,
    }
}