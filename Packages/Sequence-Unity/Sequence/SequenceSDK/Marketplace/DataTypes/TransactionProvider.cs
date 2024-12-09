using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<TransactionOnRampProvider>))]
    public enum TransactionOnRampProvider
    {
        unknown,
        sardine,
        transak,
    }
}