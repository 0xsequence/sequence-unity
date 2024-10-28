using System;

namespace Sequence.EmbeddedWallet
{
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