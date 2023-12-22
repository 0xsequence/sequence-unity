using System;

namespace Sequence.WaaS
{
    [Serializable]
    public class SignMessageReturn
    {
        public string message;
        public string signature;
        
        public SignMessageReturn(string message, string signature)
        {
            this.message = message;
            this.signature = signature;
        }
    }
}