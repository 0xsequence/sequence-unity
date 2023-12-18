using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sequence.Extensions;

namespace Sequence.Core
{
    public class Hash 
    {
        public static readonly int HashLength = 32;
        public byte[] Bytes { get; private set; }

        public Hash()
        {
            Bytes = new byte[HashLength];
        }

        public Hash(byte[] b)
        {
            int length = Mathf.Min(HashLength, b.Length);
            Bytes = new byte[HashLength];
            for (int i = 0; i < length; i++)
            {
                Bytes[i] = b[i];
            }
        }

        public static implicit operator byte[](Hash hash)
        {
            return hash.Bytes;
        }

        public static implicit operator string(Hash hash)
        {
            return hash.ToString();
        }

        public override string ToString()
        {
            return Bytes.ByteArrayToHexStringWithPrefix();
        }
    }
}
