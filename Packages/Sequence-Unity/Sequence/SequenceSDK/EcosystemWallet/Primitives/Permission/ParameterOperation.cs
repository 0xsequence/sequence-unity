using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    [JsonConverter(typeof(EnumConverter<BehaviourOnError>))]
    public enum ParameterOperation : byte
    {
        equal = 0x00,
        notEqual = 0x01,
        greaterThanOrEqual = 0x02,
        lessThanOrEqual = 0x03
    }
}