using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    public class RegisterSessionResponse
    {
        public Session session;
        public Response<IntentResponseSessionOpened> response;
        
        public RegisterSessionResponse(Session session, Response<IntentResponseSessionOpened> response)
        {
            this.session = session;
            this.response = response;
        }
    }
}