using System;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class SardineRegionState
    {
        public string code;
        public string name;
        public bool isAllowedOnRamp;
        public bool isAllowedOnNFT;

        public SardineRegionState(string code, string name, bool isAllowedOnRamp, bool isAllowedOnNft)
        {
            this.code = code;
            this.name = name;
            this.isAllowedOnRamp = isAllowedOnRamp;
            isAllowedOnNFT = isAllowedOnNft;
        }
    }
}