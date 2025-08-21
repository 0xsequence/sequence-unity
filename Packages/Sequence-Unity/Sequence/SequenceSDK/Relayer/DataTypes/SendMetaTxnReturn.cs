using UnityEngine.Scripting;

namespace Sequence.Relayer
{
    [Preserve]
    public class SendMetaTxnReturn
    {
        public bool status;
        public string txnHash;
    }
}