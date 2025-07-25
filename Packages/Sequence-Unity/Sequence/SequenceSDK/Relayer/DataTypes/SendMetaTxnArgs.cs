namespace Sequence.Relayer
{
    public class SendMetaTxnArgs
    {
        public MetaTxn call;
        public string quote;
        public int projectID;
        public IntentPrecondition[] preconditions;

        public SendMetaTxnArgs(MetaTxn call, string quote = null, int projectID = -1, IntentPrecondition[] preconditions = null)
        {
            this.call = call;
            this.quote = quote;
            this.projectID = projectID;
            this.preconditions = preconditions;
        }
    }
}