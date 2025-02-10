using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    internal class SardineSupportedRegionsResponse
    {
        public SardineRegion[] regions;
        
        [Preserve]
        public SardineSupportedRegionsResponse(SardineRegion[] regions)
        {
            this.regions = regions;
        }
    }
}