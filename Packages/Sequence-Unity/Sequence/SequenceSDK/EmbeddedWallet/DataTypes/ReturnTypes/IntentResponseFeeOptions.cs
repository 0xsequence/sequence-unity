using System;
using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentResponseFeeOptions
    {
        public FeeOption[] feeOptions;
        public string feeQuote;

        [Preserve]
        public IntentResponseFeeOptions(FeeOption[] feeOptions, string feeQuote)
        {
            this.feeOptions = feeOptions;
            this.feeQuote = feeQuote;
        }
    }
}