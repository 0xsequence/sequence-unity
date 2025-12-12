using UnityEngine.Scripting;

namespace Sequence.Relayer
{
    [Preserve]
    public class IntentPrecondition
    {
        public string type;
        public string chainId;
        public string data;
    }
}