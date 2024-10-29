using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class FeeOptionReturn
    {
        public FeeOption FeeOption;
        public bool InWallet;

        public FeeOptionReturn(FeeOption feeOption, bool inWallet)
        {
            FeeOption = feeOption;
            InWallet = inWallet;
        }
    }
}