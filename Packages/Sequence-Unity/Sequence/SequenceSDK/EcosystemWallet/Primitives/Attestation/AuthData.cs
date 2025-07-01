using System;
using System.Numerics;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class AuthData
    {
        public string redirectUrl;
        public BigInteger issuedAt;

        [Preserve]
        public AuthData(string redirectUrl, BigInteger issuedAt)
        {
            this.redirectUrl = redirectUrl;
            this.issuedAt = issuedAt;
        }

        public byte[] Encode()
        {
            byte[] redirectUrlLength = ByteArrayExtensions.ByteArrayFromNumber(redirectUrl.Length, 3);
            byte[] redirectUrlBytes = redirectUrl.ToByteArray();
            return ByteArrayExtensions.ConcatenateByteArrays(redirectUrlLength, redirectUrlBytes, issuedAt.ByteArrayFromNumber(8));
        }
    }
}