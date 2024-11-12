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

    public static class ContractTypeExtensions
    {
        public static ContractType AsMarketplaceContractType(this Sequence.ContractType contractType)
        {
            switch (contractType)
            {
                case Sequence.ContractType.ERC20:
                    return ContractType.ERC20;
                case Sequence.ContractType.ERC721:
                    return ContractType.ERC721;
                case Sequence.ContractType.ERC1155:
                    return ContractType.ERC1155;
                default:
                    return ContractType.UNKNOWN;
            }
        }
    }
}