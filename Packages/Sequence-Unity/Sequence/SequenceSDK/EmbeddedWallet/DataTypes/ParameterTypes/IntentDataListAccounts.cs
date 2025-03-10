using System;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentDataListAccounts
    {
        public string wallet;
        
        [Preserve]
        public IntentDataListAccounts(string walletAddress)
        {
            this.wallet = walletAddress;
        }
    }
}