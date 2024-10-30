using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentResponseSignedMessage
    {
        public string signature;
        public string message;
        
        public IntentResponseSignedMessage(string message, string signature)
        {
            this.message = message;
            this.signature = signature;
        }
    }
}