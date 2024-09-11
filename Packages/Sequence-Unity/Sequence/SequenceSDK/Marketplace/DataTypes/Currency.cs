using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class Currency
    {
        public uint id;
        public uint chainId;
        public string contractAddress;

        public Currency(uint id, uint chainId, string contractAddress)
        {
            this.id = id;
            this.chainId = chainId;
            this.contractAddress = contractAddress;
        }
    }
}