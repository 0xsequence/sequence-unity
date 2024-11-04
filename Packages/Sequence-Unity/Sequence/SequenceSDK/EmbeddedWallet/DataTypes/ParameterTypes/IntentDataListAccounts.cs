using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

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