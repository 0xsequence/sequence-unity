using System;
using System.Text;
using UnityEngine;

namespace Sequence.EmbeddedWallet
{
    public static class JwtHelper
    {
        [Serializable]
        private class JwtPayload
        {
            public int partner_id;
            public string wallet;
        }

        public static Address GetWalletAddressFromJwt(string jwtToken)
        {
            string[] parts = jwtToken.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid JWT format");
            }

            string payloadBase64 = PadToBase64(parts[1]);
            byte[] payloadBytes = Convert.FromBase64String(payloadBase64);
            string payloadJson = Encoding.UTF8.GetString(payloadBytes);

            JwtPayload payload;
            try
            {
                payload = JsonUtility.FromJson<JwtPayload>(payloadJson);
            }
            catch (Exception ex)
            {
                throw new Exception("JWT payload decoding failed: " + ex.Message);
            }

            return new Address(payload.wallet);
        }

        private static string PadToBase64(string value)
        {
            int length = value.Length;
            int padLength = 4 - length % 4;
            if (padLength < 4)
            {
                value += new string('=', padLength);
            }

            return value;
        }
    }
}