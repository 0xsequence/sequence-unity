using System.Collections.Generic;
using System.Numerics;

namespace Sequence
{
    public class TypedData
    {
#nullable enable
        public TypedDataDomain domain;
        public Dictionary<string, TypedDataField[]> types;
        public Dictionary<string, dynamic> message;
        public string? primaryType;

        public TypedData(TypedDataDomain domain, Dictionary<string, TypedDataField[]> types, Dictionary<string, dynamic> message)
        {
            this.domain = domain;
            this.types = types;
            this.message = message;
        }
#nullable disable
    }

    public class TypedDataDomain
    {
#nullable enable
        public string? name;
        public string? version;
        /// <summary>
        /// BigNumber
        /// </summary>
        public BigInteger? chainId;
        public string? verifyingContract;
        /// <summary>
        /// WARNING this may not behave as expected. Does sequence.js return a string or byte array here?
        /// </summary>
        public string? salt;

#nullable disable
    }

    public class TypedDataField
    {
        public string name;
        public string type;

        public TypedDataField(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }
}