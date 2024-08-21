using System;
using Newtonsoft.Json;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentDataListAccounts
    {
        public string wallet;
        
        public IntentDataListAccounts(string walletAddress)
        {
            this.wallet = walletAddress;
        }
    }
}