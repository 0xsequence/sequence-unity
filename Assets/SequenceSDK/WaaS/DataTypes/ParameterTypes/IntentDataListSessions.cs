using System;

namespace SequenceSDK.WaaS
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