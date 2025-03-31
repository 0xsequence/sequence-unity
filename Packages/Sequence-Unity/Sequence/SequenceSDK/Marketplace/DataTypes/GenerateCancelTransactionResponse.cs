using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GenerateCancelTransactionResponse
    {
        public Step[] steps;
        
        [Preserve]
        public GenerateCancelTransactionResponse(Step[] steps)
        {
            this.steps = steps;
        }
    }
}