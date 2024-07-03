using System;

namespace Sequence.Integrations.Transak
{
    [Serializable]
    public class SupportedCountriesResponse
    {
        public SupportedCountry[] response;

        public SupportedCountriesResponse(SupportedCountry[] response)
        {
            this.response = response;
        }
    }
}