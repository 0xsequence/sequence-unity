using System;
using Sequence;
using UnityEngine;

namespace SequenceSDK.WaaS
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

            byte[] payloadBytes = Convert.FromBase64String(parts[1]);
            string payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);

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
    }
}