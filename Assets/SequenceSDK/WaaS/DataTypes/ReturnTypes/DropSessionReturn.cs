using System;

namespace Sequence.WaaS
{
    [Serializable]
    public class DropSessionReturn
    {
        public bool ok { get; private set; }

        public DropSessionReturn(bool ok)
        {
            this.ok = ok;
        }
    }
}