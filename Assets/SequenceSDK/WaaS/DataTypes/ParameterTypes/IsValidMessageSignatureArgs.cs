namespace Sequence.WaaS
{
    [System.Serializable]
    public class IsValidMessageSignatureArgs
    {
        public string chainId;
        public string walletAddress;
        public string message;
        public string signature;

        public IsValidMessageSignatureArgs(string chainId, string walletAddress, string message, string signature)
        {
            this.chainId = chainId;
            this.walletAddress = walletAddress;
            this.message = message;
            this.signature = signature;
        }
    }
}