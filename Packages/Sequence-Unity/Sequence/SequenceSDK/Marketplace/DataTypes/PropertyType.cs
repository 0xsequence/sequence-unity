using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<PropertyType>))]
    public enum PropertyType
    {
        INT,
        STRING,
        ARRAY,
        GENERIC,
    }
}