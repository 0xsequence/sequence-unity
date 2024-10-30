using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class FeeOptionsResponse
    {
        public FeeOptionReturn[] FeeOptions;
        public string FeeQuote;

        public FeeOptionsResponse(FeeOptionReturn[] feeOptions, string feeQuote)
        {
            this.FeeOptions = feeOptions;
            this.FeeQuote = feeQuote;
        }
    }
}