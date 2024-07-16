using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class FeeOptionsResponse
    {
        public FeeOptionReturn[] FeeOptions { get; private set; }
        public string FeeQuote { get; private set; }

        public FeeOptionsResponse(FeeOptionReturn[] feeOptions, string feeQuote)
        {
            this.FeeOptions = feeOptions;
            this.FeeQuote = feeQuote;
        }
    }
}