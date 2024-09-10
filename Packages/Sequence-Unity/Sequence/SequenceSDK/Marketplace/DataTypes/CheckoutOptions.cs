using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class CheckoutOptions
    {
        public TransactionCrypto crypto;
        public TransactionProvider[] swap;
        public TransactionProvider[] nftCheckout;
        public TransactionProvider[] onRamp;

        public CheckoutOptions(TransactionCrypto crypto, TransactionProvider[] swap, TransactionProvider[] nftCheckout, TransactionProvider[] onRamp)
        {
            this.crypto = crypto;
            this.swap = swap;
            this.nftCheckout = nftCheckout;
            this.onRamp = onRamp;
        }
    }
}