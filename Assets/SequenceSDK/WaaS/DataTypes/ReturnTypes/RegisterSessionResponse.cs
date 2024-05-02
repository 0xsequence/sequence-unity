using Sequence.WaaS;

namespace SequenceSDK.WaaS
{
    public class RegisterSessionResponse
    {
        public WaaSSession session;
        public Response<IntentResponseSessionOpened> response;
        
        public RegisterSessionResponse(WaaSSession session, Response<IntentResponseSessionOpened> response)
        {
            this.session = session;
            this.response = response;
        }
    }
}