using System;
using System.Numerics;
using Sequence.EcosystemWallet.Primitives.Common;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Preserve]
    [Serializable]
    public class AuthData
    {
        public string redirectUrl;
        public BigInt issuedAt;

        [Preserve]
        public AuthData(string redirectUrl, BigInt issuedAt)
        {
            this.redirectUrl = redirectUrl;
            this.issuedAt = issuedAt;
        }

        public byte[] Encode()
        {
            byte[] redirectUrlLength = ByteArrayExtensions.ByteArrayFromNumber(redirectUrl.Length, 3);
            byte[] redirectUrlBytes = redirectUrl.ToByteArray();
            return ByteArrayExtensions.ConcatenateByteArrays(redirectUrlLength, redirectUrlBytes, issuedAt.Value.ByteArrayFromNumber(8));
        }
    }
}