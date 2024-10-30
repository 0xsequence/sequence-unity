using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
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