using System;
using System.Text;
using UnityEngine;

namespace Sequence.Config
{
    internal static class JwtHelper
    {
        internal static ConfigJwt GetConfigJwt(string jwt)
        {
            string payloadJson = GetJwtPayloadJson(jwt);
            ConfigJwt payload;
            try
            {
                payload = JsonUtility.FromJson<ConfigJwt>(payloadJson);
                CheckValidConfigJwt(payload);
            }
            catch (Exception ex)
            {
                throw new Exception("Config Key decoding failed: " + ex.Message + " Please double check that you've input the key correctly then contact Sequence support.");
            }

            return payload;
        }
        
        private static string GetJwtPayloadJson(string jwt)
        {
            string[] parts = jwt.Split('.');
            if (parts.Length != 1)
            {
                throw new ArgumentException("Invalid Config Key format.");
            }

            string payloadBase64 = PadToBase64(parts[0]);
            byte[] payloadBytes = Convert.FromBase64String(payloadBase64);
            string payloadJson = Encoding.UTF8.GetString(payloadBytes);
            return payloadJson;
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
        
        private static void CheckValidConfigJwt(ConfigJwt payload)
        {
            if (string.IsNullOrWhiteSpace(payload.idpRegion))
            {
                throw new Exception("Config JWT missing idpRegion.");
            }
            
            if (string.IsNullOrWhiteSpace(payload.identityPoolId))
            {
                throw new Exception("Config JWT missing identityPoolId.");
            }
            
            if (string.IsNullOrWhiteSpace(payload.keyId))
            {
                throw new Exception("Config JWT missing keyId.");
            }
            
            if (string.IsNullOrWhiteSpace(payload.rpcServer))
            {
                throw new Exception("Config JWT missing rpcServer.");
            }
            
            if (string.IsNullOrWhiteSpace(payload.kmsRegion))
            {
                throw new Exception("Config JWT missing kmsRegion.");
            }
            
            if (string.IsNullOrWhiteSpace(payload.projectId.ToString()))
            {
                throw new Exception("Config JWT missing projectId.");
            }
        }
    }
}