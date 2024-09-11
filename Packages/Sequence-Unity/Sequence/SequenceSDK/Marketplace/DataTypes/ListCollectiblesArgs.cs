using System;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class ListCollectiblesArgs
    {
        public string contractAddress;
        public CollectiblesFilter filter;
        public Page page;

        public ListCollectiblesArgs(string contractAddress, CollectiblesFilter filter = null, Page page = null)
        {
            this.contractAddress = contractAddress;
            this.filter = filter;
            this.page = page;
        }
    }
}