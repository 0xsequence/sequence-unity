using System.Numerics;

namespace Sequence.EcosystemWallet
{
    public struct SendWalletTransactionArgs
    {
        public BigInteger chainId;
        public Address address;
        public TransactionRequest transactionRequest;
    }

    public struct TransactionRequest
    {
        public Address to;
        public BigInteger value;
        public BigInteger gasLimit;
        public string data;
    }

    public struct SendWalletTransactionResponse
    {
        public string transactionHash;
        public string walletAddress;
    }
}