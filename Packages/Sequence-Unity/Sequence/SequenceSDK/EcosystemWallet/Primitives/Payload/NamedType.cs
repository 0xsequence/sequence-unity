using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Nethereum.ABI.EIP712;
using Sequence.Utils;
using UnityEngine;

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
        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is NamedType))
            {
                return false;
            }
            
            NamedType other = (NamedType)obj;
            return name.Equals(other.name) && type.Equals(other.type);
        }
        
        public override string ToString()
        {
            return $"{type} {name}";
        }
    }
}