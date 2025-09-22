using UnityEngine.Scripting;

namespace Sequence
{
    [System.Serializable]
    public class PingReturn
    {
        public bool status;

        [Preserve]
        public PingReturn(bool status)
        {
            this.status = status;
        }
    }
}