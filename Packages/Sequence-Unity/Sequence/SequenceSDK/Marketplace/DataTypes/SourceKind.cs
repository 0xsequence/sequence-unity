using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<SourceKind>))]
    public enum SourceKind
    {
        unknown,
        external,
        sequence_marketplace_v1,
        sequence_marketplace_v2,
    }
}