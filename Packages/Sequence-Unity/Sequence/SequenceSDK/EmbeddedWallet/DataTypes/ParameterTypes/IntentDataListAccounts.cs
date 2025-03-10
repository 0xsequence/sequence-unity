using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentDataListAccounts
    {
        public string wallet;
        
        [UnityEngine.Scripting.Preserve]
        public IntentDataListAccounts(string walletAddress)
        {
            this.wallet = walletAddress;
        }
    }
}