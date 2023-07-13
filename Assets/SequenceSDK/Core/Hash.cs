using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sequence.Extensions;

namespace Sequence.Core
{
    public class Hash 
    {
        public static readonly int HashLength = 32;
        byte[] Value;

        public Hash()
        {
            Value = new byte[HashLength];
        }

        public Hash(byte[] b)
        {
            int length = Mathf.Min(HashLength, b.Length);
            Value = new byte[HashLength];
            for (int i = 0; i < length; i++)
            {
                Value[i] = b[i];
            }
        }

        public static implicit operator byte[](Hash hash)
        {
            return hash.Value;
        }

        public static implicit operator string(Hash hash)
        {
            return hash.ToString();
        }

        public override string ToString()
        {
            return Value.ByteArrayToHexString();
        }
    }
}
