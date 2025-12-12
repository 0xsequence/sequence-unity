using UnityEngine.Scripting;

namespace Sequence.Relayer
{
    [Preserve]
    public class MetaTxnReceiptLog
    {
        public string address;
        public string data;
        public string[] topics;
    }
}