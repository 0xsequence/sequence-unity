using Sequence.EmbeddedWallet;

namespace Sequence.Marketplace
{
    public enum WalletKind
    {
        unknown,
        sequence
    }
    
    public static class WalletKindExtensions {
        public static WalletKind GetWalletKind(this IWallet wallet)
        {
            if (wallet is SequenceWallet)
            {
                return WalletKind.sequence;
            }

            return WalletKind.unknown;
        }
    }
}