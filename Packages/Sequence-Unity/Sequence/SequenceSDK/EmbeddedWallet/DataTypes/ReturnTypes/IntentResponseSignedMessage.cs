using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentResponseSignedMessage
    {
        public string signature;
        public string message;
        
        [UnityEngine.Scripting.Preserve]
        public IntentResponseSignedMessage(string message, string signature)
        {
            this.message = message;
            this.signature = signature;
        }
    }
}