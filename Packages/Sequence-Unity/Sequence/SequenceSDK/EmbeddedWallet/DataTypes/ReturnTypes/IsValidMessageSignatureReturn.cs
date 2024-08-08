namespace Sequence.EmbeddedWallet
{
    [System.Serializable]
    public class IsValidMessageSignatureReturn
    {
        public bool IsValid;
        public IsValidMessageSignatureReturn(bool isValid)
        {
            IsValid = isValid;
        }
    }



}