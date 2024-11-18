using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class ListCollectiblesArgs
    {
        public OrderSide side;
        public string contractAddress;
        public CollectiblesFilter filter;
        public Page page;

        [Preserve]
        internal ListCollectiblesArgs(OrderSide side, string contractAddress, CollectiblesFilter filter = null, Page page = null)
        {
            this.side = side;
            this.contractAddress = contractAddress;
            this.filter = filter;
            this.page = page;
        }
    }
}