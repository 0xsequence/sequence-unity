using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.EmbeddedWallet;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(EnumConverter<WalletKind>))]
    public enum WalletKind
    {
        unknown,
        sequence,
        unspecified,
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