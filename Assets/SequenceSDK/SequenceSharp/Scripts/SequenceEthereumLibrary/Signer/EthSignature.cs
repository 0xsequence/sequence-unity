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
using System;

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

                R = r.ToBytes();//new BigInteger(1, r.ToBytes()).ToByteArrayUnsigned();
                S = s.ToBytes();//new BigInteger(1, s.ToBytes()).ToByteArrayUnsigned();
                V = new BigInteger(1, v).ToByteArrayUnsigned();

                return GetSignatureString();
            }
            return "";
        }

        public static SecpRecoverableECDSASignature GetSignature(string signature)
        {
            UnityEngine.Debug.Log(signature);
            byte[] sig = SequenceCoder.HexStringToByteArray(signature);

            byte[] _rs = new byte[64];
            Array.Copy(sig, 0, _rs, 0, 64);
            //byte[] v = new[] { (byte)(recId + 27) };
            byte[] _v = new byte[1];
            Array.Copy(sig, 64, _v,0, 1);
            int recId = Convert.ToInt32(_v[0])-27;

            SecpRecoverableECDSASignature recoverable;

            bool created = SecpRecoverableECDSASignature.TryCreateFromCompact(_rs, recId, out recoverable);

            if(created)
            {
                return recoverable;
            }
            return null;

        }

        

        public static string GetSignatureString()
        {
            return SequenceCoder.ByteArrayToHexString(R) + SequenceCoder.ByteArrayToHexString(S) + SequenceCoder.ByteArrayToHexString(V);
        }
    }

}