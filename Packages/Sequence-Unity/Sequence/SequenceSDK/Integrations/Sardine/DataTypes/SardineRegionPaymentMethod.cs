using System;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class SardineRegionPaymentMethod
    {
        public string name;
        public bool isAllowedOnRamp;
        public bool isAllowedOnNFT;
        public string[] subTypes;
        public string type;
        public string subType;

        public SardineRegionPaymentMethod(string name, bool isAllowedOnRamp, bool isAllowedOnNft, string[] subTypes, string type, string subType)
        {
            this.name = name;
            this.isAllowedOnRamp = isAllowedOnRamp;
            isAllowedOnNFT = isAllowedOnNft;
            this.subTypes = subTypes;
            this.type = type;
            this.subType = subType;
        }
    }
}