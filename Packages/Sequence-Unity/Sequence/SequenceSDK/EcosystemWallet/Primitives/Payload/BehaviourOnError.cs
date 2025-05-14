using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    [JsonConverter(typeof(EnumConverter<BehaviourOnError>))]
    public enum BehaviourOnError
    {
        ignore,
        revert,
        abort
    }
}