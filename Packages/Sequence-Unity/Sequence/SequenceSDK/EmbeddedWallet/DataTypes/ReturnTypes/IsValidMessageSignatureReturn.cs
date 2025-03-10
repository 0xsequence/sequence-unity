using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [System.Serializable]
    public class IsValidMessageSignatureReturn
    {
        public bool isValid;
        
        [UnityEngine.Scripting.Preserve]
        public IsValidMessageSignatureReturn(bool IsValid)
        {
             isValid =  IsValid;
        }
    }
}