using System;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class BaseIntent
    {
        public string code { get; private set; }
        public uint issued { get; private set; }
        public uint expires { get; private set; }

        public BaseIntent(string code, uint timeBeforeExpiry)
        {
            this.code = code;
            this.issued = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.expires = this.issued + timeBeforeExpiry;
        }
    }
}