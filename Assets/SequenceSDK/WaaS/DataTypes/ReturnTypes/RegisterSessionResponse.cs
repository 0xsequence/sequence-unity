using Sequence.WaaS;

namespace SequenceSDK.WaaS
{
    public class RegisterSessionResponse
    {
        public WaaSSession session { get; private set; }
        public IntentResponse<IntentResponseSessionOpened> response { get; private set; }
        
        public RegisterSessionResponse(WaaSSession session, IntentResponse<IntentResponseSessionOpened> response)
        {
            this.session = session;
            this.response = response;
        }
    }
}