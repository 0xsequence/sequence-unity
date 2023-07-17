using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Core.V2.Signature
{
    public static class SignatureDecoder 
    {
        public static RegularSignature DecodeRegularSignature(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            throw new NotImplementedException();
        }

        public static NoChainIDSignature DecodeNoChainIDSignature(byte[] data)
        {
            throw new NotImplementedException();
        }

        public static ChainedSignature DecodeChainedSignature(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
