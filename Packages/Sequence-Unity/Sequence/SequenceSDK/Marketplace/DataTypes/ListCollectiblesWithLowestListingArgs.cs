using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCollectiblesWithLowestListingArgs
    {
        public string contractAddress;
        public CollectiblesFilter filter;
        public Page page;

        public ListCollectiblesWithLowestListingArgs(string contractAddress, CollectiblesFilter filter = null, Page page = null)
        {
            this.contractAddress = contractAddress;
            this.filter = filter;
            this.page = page;
        }
    }
}