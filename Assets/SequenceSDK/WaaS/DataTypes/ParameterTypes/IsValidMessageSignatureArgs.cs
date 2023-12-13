namespace Sequence.WaaS
{
    [System.Serializable]
    public class IsValidMessageSignatureArgs
    {
        public string chainId;
        public string walletAddress;
        public string message;
        public string signature;

        public IsValidMessageSignatureArgs(Chain chain, string walletAddress, string message, string signature)
        {
            this.chainId = ((int)chain).ToString();
            this.walletAddress = walletAddress;
            this.message = message;
            this.signature = signature;
        }
    }
}