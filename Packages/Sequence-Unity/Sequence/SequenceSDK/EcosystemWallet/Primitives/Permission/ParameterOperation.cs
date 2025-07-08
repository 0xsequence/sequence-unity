using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    [JsonConverter(typeof(EnumConverter<BehaviourOnError>))]
    public enum ParameterOperation
    {
        equal = 0,
        notEqual = 1,
        greaterThanOrEqual = 2,
        lessThanOrEqual = 3
    }
}