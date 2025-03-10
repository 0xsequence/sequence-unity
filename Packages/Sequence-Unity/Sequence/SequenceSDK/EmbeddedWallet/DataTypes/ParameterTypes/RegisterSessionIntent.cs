using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    public class RegisterSessionIntent
    {
        public IntentPayload intent;
        public string friendlyName;
        
        [UnityEngine.Scripting.Preserve]
        public RegisterSessionIntent(string friendlyName, IntentPayload intent)
        {
            this.friendlyName = friendlyName;
            this.intent = intent;
        }
    }
}