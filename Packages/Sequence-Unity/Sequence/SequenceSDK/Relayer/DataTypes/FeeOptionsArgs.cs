using UnityEngine.Scripting;

namespace Sequence.Relayer
{
    [Preserve]
    public class FeeOptionsArgs
    {
        public Address wallet;
        public string to;
        public string data;
        public bool simulate;

        public FeeOptionsArgs(Address wallet, string to, string data, bool simulate = false)
        {
            this.wallet = wallet;
            this.to = to;
            this.data = data;
            this.simulate = simulate;
        }
    }
}