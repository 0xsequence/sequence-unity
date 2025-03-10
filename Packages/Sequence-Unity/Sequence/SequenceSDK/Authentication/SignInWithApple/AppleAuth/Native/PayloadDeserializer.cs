using AppleAuth.Interfaces;
using Newtonsoft.Json;

namespace AppleAuth.Native
{
    public class PayloadDeserializer : IPayloadDeserializer
    {
        public ICredentialStateResponse DeserializeCredentialStateResponse(string payload)
        {
            return JsonConvert.DeserializeObject<CredentialStateResponse>(payload);
        }

        public ILoginWithAppleIdResponse DeserializeLoginWithAppleIdResponse(string payload)
        {
            return JsonConvert.DeserializeObject<LoginWithAppleIdResponse>(payload);
        }
    }
}