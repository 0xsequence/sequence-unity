using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class CheckoutOptions
    {
        public TransactionCrypto crypto;
        public TransactionSwapProvider[] swap;
        public TransactionNFTCheckoutProvider[] nftCheckout;
        public TransactionOnRampProvider[] onRamp;

        [Preserve]
        public CheckoutOptions(TransactionCrypto crypto, TransactionSwapProvider[] swap, TransactionNFTCheckoutProvider[] nftCheckout, TransactionOnRampProvider[] onRamp)
        {
            this.crypto = crypto;
            this.swap = swap;
            this.nftCheckout = nftCheckout;
            this.onRamp = onRamp;
        }
    }
}