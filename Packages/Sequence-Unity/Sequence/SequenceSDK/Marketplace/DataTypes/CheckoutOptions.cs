using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class CheckoutOptions
    {
        public TransactionCrypto crypto;
        public TransactionProvider[] swap;
        public TransactionProvider[] nftCheckout;
        public TransactionProvider[] onRamp;

        [Preserve]
        public CheckoutOptions(TransactionCrypto crypto, TransactionProvider[] swap, TransactionProvider[] nftCheckout, TransactionProvider[] onRamp)
        {
            this.crypto = crypto;
            this.swap = swap;
            this.nftCheckout = nftCheckout;
            this.onRamp = onRamp;
        }
    }
}