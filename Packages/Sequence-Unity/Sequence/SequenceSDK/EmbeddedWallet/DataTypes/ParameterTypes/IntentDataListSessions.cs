using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentDataListSessions
    {
        public string wallet;
        
        public IntentDataListSessions(string walletAddress)
        {
            this.wallet = walletAddress;
        }
    }
}