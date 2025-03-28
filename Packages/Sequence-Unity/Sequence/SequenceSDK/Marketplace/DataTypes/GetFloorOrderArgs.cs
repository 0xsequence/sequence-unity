using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class GetFloorOrderArgs
    {
        public string contractAddress;
        public CollectiblesFilter filter;

        [Preserve]
        public GetFloorOrderArgs(string contractAddress, CollectiblesFilter filter = null)
        {
            this.contractAddress = contractAddress;
            this.filter = filter;
        }
    }
}