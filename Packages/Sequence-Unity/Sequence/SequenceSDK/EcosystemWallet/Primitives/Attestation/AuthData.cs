using System;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class AuthData
    {
        public string redirectUrl;

        [Preserve]
        public AuthData(string redirectUrl)
        {
            this.redirectUrl = redirectUrl;
        }

        public byte[] Encode()
        {
            byte[] redirectUrlLength = ByteArrayExtensions.ByteArrayFromNumber(redirectUrl.Length, 3);
            byte[] redirectUrlBytes = redirectUrl.ToByteArray();
            return ByteArrayExtensions.ConcatenateByteArrays(redirectUrlLength, redirectUrlBytes);
        }
    }
}