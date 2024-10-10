using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<TransactionCrypto>))]
    public enum TransactionCrypto
    {
        none,
        partially,
        all
    }
}