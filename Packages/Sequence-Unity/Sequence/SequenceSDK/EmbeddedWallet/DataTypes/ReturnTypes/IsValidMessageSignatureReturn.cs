namespace Sequence.EmbeddedWallet
{
    [System.Serializable]
    public class IsValidMessageSignatureReturn
    {
        public bool IsValid { get; set; }
        public IsValidMessageSignatureReturn(bool isValid)
        {
            IsValid = isValid;
        }
    }



}