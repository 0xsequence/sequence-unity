namespace SequenceSDK.WaaS
{
    public class RegisterSessionIntent
    {
        public IntentPayload intent { get; private set; }
        public string friendlyName { get; private set; }
        
        public RegisterSessionIntent(string friendlyName, IntentPayload intent)
        {
            this.friendlyName = friendlyName;
            this.intent = intent;
        }
    }
}