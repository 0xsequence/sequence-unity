using System;

namespace Sequence.Marketplace
{
    [Serializable]
    public class Signature
    {
        public Domain domain;
        public object types;
        public string primaryType;
        public object value;

        public Signature(Domain domain, object types, string primaryType, object value)
        {
            this.domain = domain;
            this.types = types;
            this.primaryType = primaryType;
            this.value = value;
        }
    }
}