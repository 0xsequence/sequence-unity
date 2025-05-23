using System;
using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<TransactionSwapProvider>))]
    public enum TransactionSwapProvider
    {
        unknown,
        [Obsolete("zerox is no longer supported")]
        zerox,
        lifi
    }
}