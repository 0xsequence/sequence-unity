namespace Sequence.WaaS
{
    [System.Serializable]
    public class IsValidMessageSignatureArgs
    {
        public uint chainId;
        public string walletAddress;
        public string message;
        public string signature;

        public IsValidMessageSignatureArgs(uint chainId, string walletAddress, string message, string signature)
        {
            this.chainId = chainId;
            this.walletAddress = walletAddress;
            this.message = message;
            this.signature = signature;
        }
    }
}