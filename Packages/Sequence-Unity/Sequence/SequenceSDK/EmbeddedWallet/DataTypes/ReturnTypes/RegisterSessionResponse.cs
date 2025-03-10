using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    public class RegisterSessionResponse
    {
        public Session session;
        public Response<IntentResponseSessionOpened> response;
        
        [UnityEngine.Scripting.Preserve]
        public RegisterSessionResponse(Session session, Response<IntentResponseSessionOpened> response)
        {
            this.session = session;
            this.response = response;
        }
    }
}