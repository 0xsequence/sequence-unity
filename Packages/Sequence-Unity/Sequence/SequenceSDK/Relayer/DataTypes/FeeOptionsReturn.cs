using UnityEngine.Scripting;

namespace Sequence.Relayer
{
    [Preserve]
    public class FeeOptionsReturn
    {
        public FeeOption[] options;
        public bool sponsored;
        public string quote;
    }
}