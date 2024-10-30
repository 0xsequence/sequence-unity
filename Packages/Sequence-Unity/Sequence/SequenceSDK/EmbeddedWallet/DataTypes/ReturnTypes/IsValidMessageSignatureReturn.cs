using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [System.Serializable]
    public class IsValidMessageSignatureReturn
    {
        public bool isValid;
        public IsValidMessageSignatureReturn(bool IsValid)
        {
             isValid =  IsValid;
        }
    }
}