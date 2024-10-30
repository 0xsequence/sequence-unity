using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentDataListSessions
    {
        public string wallet;
        
        [Preserve]
        public IntentDataListSessions(string walletAddress)
        {
            this.wallet = walletAddress;
        }
    }
}