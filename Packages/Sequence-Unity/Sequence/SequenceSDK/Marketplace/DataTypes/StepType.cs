using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<StepType>))]
    public enum StepType
    {
        unknown,
        tokenApproval,
        buy,
        sell,
        createListing,
        createOffer,
        signEIP712,
        signEIP191,
    }
}