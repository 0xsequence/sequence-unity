using System;
using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentResponseSignedMessage
    {
        public string signature;
        public string message;
        
        [Preserve]
        public IntentResponseSignedMessage(string message, string signature)
        {
            this.message = message;
            this.signature = signature;
        }
    }
}