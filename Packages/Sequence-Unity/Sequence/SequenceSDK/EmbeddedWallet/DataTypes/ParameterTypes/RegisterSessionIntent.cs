using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    public class RegisterSessionIntent
    {
        public IntentPayload intent;
        public string friendlyName;
        
        [Preserve]
        public RegisterSessionIntent(string friendlyName, IntentPayload intent)
        {
            this.friendlyName = friendlyName;
            this.intent = intent;
        }
    }
}