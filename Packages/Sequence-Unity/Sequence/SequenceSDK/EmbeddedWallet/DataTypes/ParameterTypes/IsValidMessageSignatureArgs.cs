using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [System.Serializable]
    public class IsValidMessageSignatureArgs
    {
        public string chainId;
        public string walletAddress;
        public string message;
        public string signature;

        [Preserve]
        public IsValidMessageSignatureArgs(Chain chain, string walletAddress, string message, string signature)
        {
            this.chainId = chain.GetChainId();
            this.walletAddress = walletAddress;
            this.message = message;
            this.signature = signature;
        }
    }
}