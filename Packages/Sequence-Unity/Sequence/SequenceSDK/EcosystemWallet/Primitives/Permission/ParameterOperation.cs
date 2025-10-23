using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    [JsonConverter(typeof(EnumConverter<ParameterOperation>))]
    public enum ParameterOperation : int
    {
        equal = 0,
        notEqual = 1,
        greaterThanOrEqual = 2,
        lessThanOrEqual = 3
    }
}