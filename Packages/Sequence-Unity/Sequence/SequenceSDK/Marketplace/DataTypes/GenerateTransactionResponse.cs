using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GenerateTransactionResponse
    {
        public Step[] steps;

        public GenerateTransactionResponse(Step[] steps)
        {
            this.steps = steps;
        }
    }
}