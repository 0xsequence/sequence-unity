namespace Sequence.EmbeddedWallet
{
    [System.Serializable]
    public class SimulateResult
    {
        public bool executed { get; private set; }
        public bool succeeded { get; private set; }
        public string result { get; private set; }
        public string reason { get; private set; }
        public uint gasUsed { get; private set; }
        public uint gasLimit { get; private set; }

        public SimulateResult(bool executed, bool succeeded, uint gasUsed, uint gasLimit, string result = null, string reason = null)
        {
            this.executed = executed;
            this.succeeded = succeeded;
            this.gasUsed = gasUsed;
            this.gasLimit = gasLimit;
            this.result = result;
            this.reason = reason;
        }
    }
}