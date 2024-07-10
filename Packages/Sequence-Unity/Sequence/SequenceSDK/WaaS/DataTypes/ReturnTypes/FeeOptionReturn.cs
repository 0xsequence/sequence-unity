using System;

namespace Sequence.WaaS
{
    [Serializable]
    public class FeeOptionReturn
    {
        public FeeOption FeeOption { get; private set; }
        public bool InWallet { get; private set; }

        public FeeOptionReturn(FeeOption feeOption, bool inWallet)
        {
            FeeOption = feeOption;
            InWallet = inWallet;
        }
    }
}