using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    public class IntentResponseValidationFinished
    {
        public bool isValid;
        
        public IntentResponseValidationFinished(bool isValid)
        {
            this.isValid = isValid;
        }
    }
}