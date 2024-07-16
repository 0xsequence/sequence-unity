using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentResponseValidationStarted
    {
        public string salt { get; private set; }
        
        public IntentResponseValidationStarted(string salt)
        {
            this.salt = salt;
        }
    }
}