using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCollectibleListingsArgs
    {
        public Address contractAddress;
        public string tokenId;
        public OrderFilter filter;
        public Page page;

        public ListCollectibleListingsArgs(Address contractAddress, string tokenId, OrderFilter filter = null, Page page = null)
        {
            this.contractAddress = contractAddress;
            this.tokenId = tokenId;
            this.filter = filter;
            this.page = page;
        }
    }
}