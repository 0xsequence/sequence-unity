using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [System.Serializable]
    public class IsValidMessageSignatureArgs
    {
        public string chainId;
        public string walletAddress;
        public string message;
        public string signature;

        [UnityEngine.Scripting.Preserve]
        public IsValidMessageSignatureArgs(Chain chain, string walletAddress, string message, string signature)
        {
            this.chainId = chain.GetChainId();
            this.walletAddress = walletAddress;
            this.message = message;
            this.signature = signature;
        }
    }
}