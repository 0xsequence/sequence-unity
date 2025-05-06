using System;

namespace Sequence.EcosystemWallet.Primitives
{
        
    [Serializable]
    public class NamedType
    {
        public string name;
        public string type;

        public NamedType(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }
}