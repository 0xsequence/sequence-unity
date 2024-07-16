using System;
using Newtonsoft.Json;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentResponseFeeOptions
    {
        public FeeOption[] feeOptions { get; private set; }
        public string feeQuote { get; private set; }

        public IntentResponseFeeOptions(FeeOption[] feeOptions, string feeQuote)
        {
            this.feeOptions = feeOptions;
            this.feeQuote = feeQuote;
        }
    }
}