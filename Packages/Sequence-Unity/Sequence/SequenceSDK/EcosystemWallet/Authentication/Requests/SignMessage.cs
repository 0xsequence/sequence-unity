using Sequence.EcosystemWallet.Primitives.Common;

namespace Sequence.EcosystemWallet.Authentication.Requests
{
    
    public struct SignMessageArgs
    {
        public Address address;
        public BigInt chainId;
        public string message;
    }

    public struct SignMessageResponse
    {
        public string signature;
        public string walletAddress;
        public string managerRequestId;
    }
}