using System;
using System.Text;
using Newtonsoft.Json;

namespace Sequence.Authentication
{
    public static class JwtHelper
    {
        public static IdTokenJwtPayload GetIdTokenJwtPayload(string jwt)
        {
            string payloadJson = GetJwtPayloadJson(jwt);
            IdTokenJwtPayload payload;
            try
            {
                payload = JsonConvert.DeserializeObject<IdTokenJwtPayload>(payloadJson);
            }
            catch (Exception ex)
            {
                throw new Exception("JWT payload decoding failed: " + ex.Message);
            }

            return payload;
        }
        
        private static string GetJwtPayloadJson(string jwt)
        {
            string[] parts = jwt.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid JWT format");
            }

            string payloadBase64 = PadToBase64(parts[1]);
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
    }
    
    [Serializable]
    public class IdTokenJwtPayload
    {
        public string iss;
        public string azp;
        public string aud;
        public string sub;
        public string email;
        public string email_verified;
        public string at_hash;
        public string nonce;
        public string name;
        public string picture;
        public string given_name;
        public string family_name;
        public string locale;
        public string iat;
        public string exp;
    }
}