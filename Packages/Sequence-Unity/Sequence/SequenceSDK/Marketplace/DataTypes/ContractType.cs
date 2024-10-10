using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    
    [JsonConverter(typeof(EnumConverter<ContractType>))]
    public enum ContractType
    {
        UNKNOWN,
        ERC20,
        ERC721,
        ERC1155
    }
}