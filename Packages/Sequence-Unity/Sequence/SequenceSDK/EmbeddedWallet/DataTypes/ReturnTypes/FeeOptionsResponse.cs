using System;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class FeeOptionsResponse
    {
        public FeeOptionReturn[] FeeOptions;
        public string FeeQuote;

        [Preserve]
        public FeeOptionsResponse(FeeOptionReturn[] feeOptions, string feeQuote)
        {
            this.FeeOptions = feeOptions;
            this.FeeQuote = feeQuote;
        }
    }
}