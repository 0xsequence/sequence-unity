using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentResponseValidationStarted
    {
        public string salt;
        
        public IntentResponseValidationStarted(string salt)
        {
            this.salt = salt;
        }
    }
}