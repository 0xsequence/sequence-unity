
using System.Collections.Generic;

namespace SequenceSharp
{
    [System.Serializable]
    public class FeeHistoryResult
    {
        public string oldestBlock;
        public List<string> baseFeePerGas;
        public List<List<string>>? reward;
    }
}