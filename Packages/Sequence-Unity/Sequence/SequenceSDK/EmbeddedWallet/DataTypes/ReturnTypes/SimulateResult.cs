using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [System.Serializable]
    public class SimulateResult
    {
        public bool executed;
        public bool succeeded;
        public string result;
        public string reason;
        public uint gasUsed;
        public uint gasLimit;

        [Preserve]
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