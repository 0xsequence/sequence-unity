namespace Sequence.EmbeddedWallet
{
    public class RegisterSessionResponse
    {
        public WaaSSession session { get; private set; }
        public Response<IntentResponseSessionOpened> response { get; private set; }
        
        public RegisterSessionResponse(WaaSSession session, Response<IntentResponseSessionOpened> response)
        {
            this.session = session;
            this.response = response;
        }
    }
}