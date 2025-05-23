using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    [JsonConverter(typeof(EnumConverter<BehaviourOnError>))]
    public enum BehaviourOnError
    {
        ignore = 0x00,
        revert = 0x01,
        abort = 0x02
    }
}