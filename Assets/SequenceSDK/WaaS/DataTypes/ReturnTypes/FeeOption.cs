using System;

namespace Sequence.WaaS
{
    [Serializable]
    public class FeeOption
    {
        public uint gasLimit { get; private set; }
        public string to { get; private set; }
        public FeeToken token { get; private set; }
        public string value { get; private set; }

        public FeeOption(uint gasLimit, string to, FeeToken token, string value)
        {
            this.gasLimit = gasLimit;
            this.to = to;
            this.token = token;
            this.value = value;
        }
    }
}