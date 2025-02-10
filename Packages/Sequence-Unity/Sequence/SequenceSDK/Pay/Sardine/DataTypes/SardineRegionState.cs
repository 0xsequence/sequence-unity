using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    public class SardineRegionState
    {
        public string code;
        public string name;
        public bool isAllowedOnRamp;
        public bool isAllowedOnNFT;

        [Preserve]
        public SardineRegionState(string code, string name, bool isAllowedOnRamp, bool isAllowedOnNft)
        {
            this.code = code;
            this.name = name;
            this.isAllowedOnRamp = isAllowedOnRamp;
            isAllowedOnNFT = isAllowedOnNft;
        }
    }
}