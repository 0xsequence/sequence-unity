namespace Sequence
{
    using System.Numerics;

    [System.Serializable]
    public class TokenBalance
    {
        public int id;
        public string contractAddress;
        public ContractType contractType;
        public string accountAddress;
        public BigInteger tokenID;
        public BigInteger balance;
        public string blockHash;
        public BigInteger blockNumber;
        public BigInteger updateID;
        public BigInteger chainId;
        public ContractInfo contractInfo;
        public TokenMetadata tokenMetadata;
    }
}