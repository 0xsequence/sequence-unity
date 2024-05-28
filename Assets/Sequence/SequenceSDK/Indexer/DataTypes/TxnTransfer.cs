namespace Sequence
{
    using System.Collections.Generic;
    using System.Numerics;

    [System.Serializable]
    public class TxnTransfer
    {
        public TxnTransferType transferType;
        public string contractAddress;
        public ContractType contractType;
        public string from;
        public string to;
        public BigInteger[] tokenIds;
        public BigInteger[] amounts;
        public int logIndex;
        public ContractInfo contractInfo;
        public Dictionary<string, TokenMetadata> tokenMetaData;
    }
}