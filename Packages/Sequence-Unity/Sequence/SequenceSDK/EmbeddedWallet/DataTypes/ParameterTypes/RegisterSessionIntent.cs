namespace Sequence.EmbeddedWallet
{
    public class RegisterSessionIntent
    {
        public IntentPayload intent;
        public string friendlyName;
        
        public RegisterSessionIntent(string friendlyName, IntentPayload intent)
        {
            this.friendlyName = friendlyName;
            this.intent = intent;
        }
    }
}