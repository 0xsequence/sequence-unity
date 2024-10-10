using System;
using Newtonsoft.Json;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class ListCollectiblesArgs
    {
        public OrderSide side;
        public string contractAddress;
        public CollectiblesFilter filter;
        public Page page;

        public ListCollectiblesArgs(OrderSide side, string contractAddress, CollectiblesFilter filter = null, Page page = null)
        {
            this.side = side;
            this.contractAddress = contractAddress;
            this.filter = filter;
            this.page = page;
        }
    }
}