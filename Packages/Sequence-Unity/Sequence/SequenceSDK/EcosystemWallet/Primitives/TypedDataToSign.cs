using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class TypedDataToSign
    {
        [Serializable]
        public class Domain
        {
            public string name;
            public string version;
            public BigInteger chainId;
            public Address verifyingContract;

            public Domain(string name, string version, BigInteger chainId, Address verifyingContract)
            {
                this.name = name;
                this.version = version;
                this.chainId = chainId;
                this.verifyingContract = verifyingContract;
            }
        }
        
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

        public Domain domain;
        public Dictionary<string, NamedType[]> types;
        public string primaryType;
        public Dictionary<string, object> message;

        [Preserve]
        public TypedDataToSign(Domain domain, Dictionary<string, NamedType[]> types, string primaryType, Dictionary<string, object> message)
        {
            this.domain = domain;
            this.types = types;
            this.primaryType = primaryType;
            this.message = message;
        }
    }
}