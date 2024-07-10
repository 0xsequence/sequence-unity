using System.Numerics;

namespace Sequence
{
    [System.Serializable]
    public class SyncStatus
    {
        public bool syncing;
        public BigInteger startingBlock;
        public BigInteger currentBlock;
        public BigInteger highestBlock;
    }
}