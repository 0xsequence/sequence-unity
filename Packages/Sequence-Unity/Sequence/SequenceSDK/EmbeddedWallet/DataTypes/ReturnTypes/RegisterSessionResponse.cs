namespace Sequence.EmbeddedWallet
{
    public class RegisterSessionResponse
    {
        public Session session { get; private set; }
        public Response<IntentResponseSessionOpened> response { get; private set; }
        
        public RegisterSessionResponse(Session session, Response<IntentResponseSessionOpened> response)
        {
            this.session = session;
            this.response = response;
        }
    }
}