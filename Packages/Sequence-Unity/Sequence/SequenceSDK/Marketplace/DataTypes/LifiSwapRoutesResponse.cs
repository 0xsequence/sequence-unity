using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    internal class LifiSwapRoutesResponse
    {
        public LifiSwapRoute[] routes;

        [Preserve]
        public LifiSwapRoutesResponse(LifiSwapRoute[] routes)
        {
            this.routes = routes;
        }
    }
}