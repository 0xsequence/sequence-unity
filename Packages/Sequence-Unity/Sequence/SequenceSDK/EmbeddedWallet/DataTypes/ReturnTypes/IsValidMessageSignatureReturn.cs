namespace Sequence.EmbeddedWallet
{
    [System.Serializable]
    public class IsValidMessageSignatureReturn
    {
        public bool isValid { get; set; }
        public IsValidMessageSignatureReturn(bool IsValid)
        {
             isValid =  IsValid;
        }
    }



}