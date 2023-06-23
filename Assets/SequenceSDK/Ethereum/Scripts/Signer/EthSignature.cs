#define HAS_SPAN
#define SECP256K1_LIB
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBitcoin.Secp256k1;
using Sequence.ABI;
using System.Text;
using System.Linq;
using Org.BouncyCastle.Math;
using System;

namespace Sequence.Signer
{
    public static class EthSignature
    {
        public static byte[] R;
        public static byte[] S;
        public static byte[] V;

        /// <summary>
        /// Signs a hashed message using an EC private key and returns the signature as a string.
        /// </summary>
        /// <param name="hashedMessage">The hashed message to sign.</param>
        /// <param name="privKey">The EC private key to use for signing.</param>
        /// <returns>The signature as a string.</returns>
        public static string Sign(byte[] hashedMessage, ECPrivKey privKey)
        {
            SecpRecoverableECDSASignature signature;
            bool signed = privKey.TrySignRecoverable(hashedMessage, out signature);

            if (signed)
            {
                // Extract the components of the signature
                byte[] sigHash64 = new byte[64];

                Scalar r, s;
                int recId;
                signature.WriteToSpanCompact(sigHash64, out recId);
                signature.Deconstruct(out r, out s, out recId);
                byte[] v = new[] { (byte)(recId + 27) };

                R = r.ToBytes();
                S = s.ToBytes();
                V = new BigInteger(1, v).ToByteArrayUnsigned();

                return GetSignatureString();
            }
            return "";
        }

        /// <summary>
        /// Signs a hashed message using an EC private key and returns the signature components (v, r, s) as strings.
        /// </summary>
        /// <param name="hashedMessage">The hashed message to sign.</param>
        /// <param name="privKey">The EC private key to use for signing.</param>
        /// <param name="chainId">The chain ID for the transaction.</param>
        /// <returns>A tuple containing the signature components (v, r, s) as strings.</returns>
        public static (string v, string r, string s) SignAndReturnVRS(byte[] hashedMessage, ECPrivKey privKey, int chainId)
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
                byte[] v = new[] { (byte)(recId +chainId*2+ 35) };

                R = r.ToBytes();//new BigInteger(1, r.ToBytes()).ToByteArrayUnsigned();
                S = s.ToBytes();//new BigInteger(1, s.ToBytes()).ToByteArrayUnsigned();
                V = new BigInteger(1, v).ToByteArrayUnsigned();

                return GetSignatureForTransaction();
            }
            return ("", "","");
        }

        /// <summary>
        /// Signs a hashed message using an EC private key and returns the signature components (v, r, s) as strings.
        /// </summary>
        /// <param name="hashedMessage">The hashed message to sign.</param>
        /// <param name="privKey">The EC private key to use for signing.</param>
        /// <returns>A tuple containing the signature components (v, r, s) as strings.</returns>
        public static (string v, string r, string s) SignAndReturnVRS(byte[] hashedMessage, ECPrivKey privKey)
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

                R = r.ToBytes();
                UnityEngine.Debug.Log("R: " + r.d0 + r.d1 + r.d2 + r.d3 + r.d4 + r.d5 + r.d6 + r.d7);
                S = s.ToBytes();
                UnityEngine.Debug.Log("S: " + s.d0 + s.d1 + s.d2 + s.d3 + s.d4 + s.d5 + s.d6 + s.d7);
                V = new BigInteger(1, v).ToByteArrayUnsigned();
                UnityEngine.Debug.Log("V: " + SequenceCoder.ByteArrayToHexString(v));

                return GetSignatureForTransaction();
            }
            return ("", "", "");
        }

        /// <summary>
        /// Retrieves a SecpRecoverableECDSASignature object from a signature string.
        /// </summary>
        /// <param name="signature">The signature string.</param>
        /// <returns>The SecpRecoverableECDSASignature object.</returns>
        public static SecpRecoverableECDSASignature GetSignature(string signature)
        {
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


        /// <summary>
        /// Converts the signature components (v, r, s) to a signature string.
        /// </summary>
        /// <returns>The signature as a string.</returns>
        public static string GetSignatureString()
        {
            return "0x"+SequenceCoder.ByteArrayToHexString(R) + SequenceCoder.ByteArrayToHexString(S) + SequenceCoder.ByteArrayToHexString(V);
        }

        /// <summary>
        /// Retrieves the signature components (v, r, s) as strings for a transaction.
        /// </summary>
        /// <returns>A tuple containing the signature components (v, r, s) as strings.</returns>
        public static (string v, string r, string s) GetSignatureForTransaction()
        {
            return (("0x" + SequenceCoder.ByteArrayToHexString(V) ,("0x" + SequenceCoder.ByteArrayToHexString(R) ), ("0x"+ SequenceCoder.ByteArrayToHexString(S))));
        }
    }

}