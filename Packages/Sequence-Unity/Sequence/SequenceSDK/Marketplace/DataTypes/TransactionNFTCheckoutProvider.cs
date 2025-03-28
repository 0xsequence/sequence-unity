using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<TransactionNFTCheckoutProvider>))]
    public enum TransactionNFTCheckoutProvider
    {
        unknown,
        sardine,
        transak,
    }
}