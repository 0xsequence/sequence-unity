using UnityEngine.Serialization;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class SignMessageArgs
    {
        public uint chainId;
        public string walletAddress;
        public string message;

        public SignMessageArgs(uint chainId, string walletAddress, string message)
        {
            this.chainId = chainId;
            this.walletAddress = walletAddress;
            this.message = message;
        }
        
        public SignMessageArgs(Chain chainId, string walletAddress, string message)
        {
            this.chainId = (uint)chainId;
            this.walletAddress = walletAddress;
            this.message = message;
        }
    }
}