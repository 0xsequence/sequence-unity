using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class FeeOptionReturn
    {
        public FeeOption FeeOption;
        public bool InWallet;

        [UnityEngine.Scripting.Preserve]
        public FeeOptionReturn(FeeOption feeOption, bool inWallet)
        {
            FeeOption = feeOption;
            InWallet = inWallet;
        }
    }
}