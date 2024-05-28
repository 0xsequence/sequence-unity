using System.Numerics;

namespace Sequence
{
    [System.Serializable]
    public class RuntimeStatus
    {
        public bool healthOK;
        public bool indexerEnabled;
        public string startTime;
        public double uptime;
        public string ver;
        public string branch;
        public string commitHash;
        public string chainID;
        public RuntimeChecks checks;
    }
}