using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Transak
{
    [Serializable]
    public class SupportedCountry
    {
        public string alpha2;
        public string alpha3;
        public bool isAllowed;
        public bool isLightKycAllowed;
        public string name;
        public string[] supportedDocuments;
        public string currencyCode;
        public Partner[] partners;

        [Preserve]
        public SupportedCountry(string alpha2, string alpha3, bool isAllowed, bool isLightKycAllowed, string name, string[] supportedDocuments, string currencyCode, Partner[] partners)
        {
            this.alpha2 = alpha2;
            this.alpha3 = alpha3;
            this.isAllowed = isAllowed;
            this.isLightKycAllowed = isLightKycAllowed;
            this.name = name;
            this.supportedDocuments = supportedDocuments;
            this.currencyCode = currencyCode;
            this.partners = partners;
        }
    }
}