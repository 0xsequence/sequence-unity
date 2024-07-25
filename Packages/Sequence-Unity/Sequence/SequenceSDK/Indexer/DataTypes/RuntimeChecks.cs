using System.Numerics;

namespace Sequence
{
    [System.Serializable]
    public class RuntimeChecks
    {
        public bool running;
        public string syncMode;
        public BigInteger lastBlockNum;
    }
}