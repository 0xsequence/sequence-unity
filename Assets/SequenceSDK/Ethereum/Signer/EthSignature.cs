#define HAS_SPAN
#define SECP256K1_LIB
using NBitcoin.Secp256k1;
using Sequence.ABI;
using System;
using System.Numerics;
using Sequence.Utils;

namespace Sequence.Signer
{
    public static class EthSignature
    {
        public static byte[] R;
        public static byte[] S;
        public static string V;

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
                BigInteger v = recId + 27;

                R = r.ToBytes();
                S = s.ToBytes();
                V = v.BigIntegerToHexString();

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
                BigInteger v = recId + chainId * 2 + 35;
                R = r.ToBytes();
                S = s.ToBytes();
                V = v.BigIntegerToHexString();

                return GetSignatureForTransaction();
            }
            return ("", "","");
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
            return "0x"+SequenceCoder.ByteArrayToHexString(R) + SequenceCoder.ByteArrayToHexString(S) + V.Replace("0x","");
        }

        /// <summary>
        /// Retrieves the signature components (v, r, s) as strings for a transaction.
        /// </summary>
        /// <returns>A tuple containing the signature components (v, r, s) as strings.</returns>
        public static (string v, string r, string s) GetSignatureForTransaction()
        {
            return (V, "0x" + SequenceCoder.ByteArrayToHexString(R).TrimStart('0'), "0x"+ SequenceCoder.ByteArrayToHexString(S).TrimStart('0'));
        }
    }

}