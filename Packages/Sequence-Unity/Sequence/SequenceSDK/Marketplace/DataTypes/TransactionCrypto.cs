using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<TransactionCrypto>))]
    public enum TransactionCrypto
    {
        unknown,
        none,
        partially,
        all
    }
}