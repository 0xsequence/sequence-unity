#define HAS_SPAN
#define SECP256K1_LIB
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBitcoin.Secp256k1;
using SequenceSharp.ABI;
using System.Text;
using System.Linq;
using Org.BouncyCastle.Math;

namespace SequenceSharp.Signer
{
    public static class EthSignature
    {
        public static byte[] R;
        public static byte[] S;
        public static byte[] V;

        public static string Sign(byte[] hashedMessage, ECPrivKey privKey)
        {
            
            SecpRecoverableECDSASignature signature;
            bool signed = privKey.TrySignRecoverable(hashedMessage, out signature);

            if (signed)
            {
                byte[] sigHash64 = new byte[64];

                Scalar r, s;
                int recId;
                signature.WriteToSpanCompact(sigHash64, out recId);
                signature.Deconstruct(out r, out s, out recId);
                byte[] v = new[] { (byte)(recId + 27) };
                R = new BigInteger(1, r.ToBytes()).ToByteArrayUnsigned();
                S = new BigInteger(1, s.ToBytes()).ToByteArrayUnsigned();
                V = new BigInteger(1, v).ToByteArrayUnsigned();

                return GetSignatureString();
            }
            return "";
        }

        

        public static string GetSignatureString()
        {
            return SequenceCoder.ByteArrayToHexString(R) + SequenceCoder.ByteArrayToHexString(S) + SequenceCoder.ByteArrayToHexString(V);
        }
    }

}