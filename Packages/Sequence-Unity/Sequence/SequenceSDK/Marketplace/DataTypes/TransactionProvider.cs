using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<TransactionProvider>))]
    public enum TransactionProvider
    {
        unknown,
        sardine,
        transak,
        zerox
    }
}