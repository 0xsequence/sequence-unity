namespace Sequence.WaaS
{
    [System.Serializable]
    public class FailedTransactionReturn
    {
        public string error { get; private set; }
        public SendTransactionArgs request { get; private set; }
        public SimulateResult[] simulations { get; private set; }

        public FailedTransactionReturn(string error, SendTransactionArgs request, SimulateResult[] simulations)
        {
            this.error = error;
            this.request = request;
            this.simulations = simulations;
        }
    }
}