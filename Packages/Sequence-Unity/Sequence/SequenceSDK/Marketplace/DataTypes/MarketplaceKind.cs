using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<MarketplaceKind>))]
    public enum MarketplaceKind
    {
        unknown,
        sequence_marketplace_v1,
        sequence_marketplace_v2,
        opensea,
        magic_eden,
        mintify,
        looks_rare,
        x2y2,
        sudo_swap,
        coinbase,
        rarible,
        nftx,
        foundation,
        manifold,
        zora,
        blur, 
        super_rare,
        okx,
        element,
        aqua_xyz,
        auranft_co,
        zerox,
        alienswap,
        payment_processor,
    }
}