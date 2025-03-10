using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentDataListSessions
    {
        public string wallet;
        
        [UnityEngine.Scripting.Preserve]
        public IntentDataListSessions(string walletAddress)
        {
            this.wallet = walletAddress;
        }
    }
}