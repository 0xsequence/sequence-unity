
using System.Collections.Generic;

namespace Sequence
{
    [System.Serializable]
    public class FeeHistoryResult
    {
        public string oldestBlock;
        public List<string> baseFeePerGas;
        public List<float> gasUsedRatio;
        public List<List<string>>? reward;
    }
}