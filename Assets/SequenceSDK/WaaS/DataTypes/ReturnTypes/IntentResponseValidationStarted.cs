using System;

namespace Sequence.WaaS
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