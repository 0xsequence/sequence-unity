namespace Sequence.EcosystemWallet
{
    public struct SignMessageArgs
    {
        public Address address;
        public string chainId;
        public string message;
    }

    public struct SignMessageResponse
    {
        public string signature;
        public string walletAddress;
        public string managerRequestId;
    }
}