using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Transak
{
    [Serializable]
    public class SupportedCountriesResponse
    {
        public SupportedCountry[] response;

        [Preserve]
        public SupportedCountriesResponse(SupportedCountry[] response)
        {
            this.response = response;
        }
    }
}