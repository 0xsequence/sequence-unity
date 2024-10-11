using System;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class SardineRegion
    {
        public string countryCode;
        public bool isAllowedOnRamp;
        public bool isAllowedOnNFT;
        public string[] isBasicKycRequired;
        public string[] isSsnRequired;
        public string name;
        public string currencyCode;
        public bool isPayrollSupported;
        public string[] supportedDocuments;
        public SardineRegionPaymentMethod[] paymentMethods;
        public SardineRegionState[] states;

        public SardineRegion(string countryCode, bool isAllowedOnRamp, bool isAllowedOnNft, string[] isBasicKycRequired, string[] isSsnRequired, string name, string currencyCode, bool isPayrollSupported, string[] supportedDocuments, SardineRegionPaymentMethod[] paymentMethods, SardineRegionState[] states)
        {
            this.countryCode = countryCode;
            this.isAllowedOnRamp = isAllowedOnRamp;
            isAllowedOnNFT = isAllowedOnNft;
            this.isBasicKycRequired = isBasicKycRequired;
            this.isSsnRequired = isSsnRequired;
            this.name = name;
            this.currencyCode = currencyCode;
            this.isPayrollSupported = isPayrollSupported;
            this.supportedDocuments = supportedDocuments;
            this.paymentMethods = paymentMethods;
            this.states = states;
        }
    }
}