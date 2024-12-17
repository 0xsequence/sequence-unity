using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GenerateTransactionResponse
    {
        public Step[] steps;

        [Preserve]
        public GenerateTransactionResponse(Step[] steps)
        {
            this.steps = steps;
        }
    }
}