namespace Sequence.Marketplace
{
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
    }

    public static class MarketplaceKindExtensions
    {
        public static string AsString(this MarketplaceKind kind)
        {
            return kind.ToString();
        }
    }
}