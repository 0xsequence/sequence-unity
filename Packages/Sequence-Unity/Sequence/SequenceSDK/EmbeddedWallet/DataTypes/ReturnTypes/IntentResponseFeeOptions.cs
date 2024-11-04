using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

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