using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.EmbeddedWallet
{
    public class ResponseSignatureValidator
    {
        private string _waasPublicKey;
        public bool PublicKeyFetched { get; private set; }
        private const string _jwksUrl = "https://waas.sequence.app/.well-known/jwks.json";
        
        public ResponseSignatureValidator()
        {
            FetchPublicKey();
        }

        private async Task FetchPublicKey()
        {
            try
            {
                using UnityWebRequest request = UnityWebRequest.Get(_jwksUrl);
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"Failed to get JWKS: {request.error}");
                }

                var json = JObject.Parse(request.downloadHandler.text);
                var key = json["keys"][0];

                string exponent = key["e"].ToString();
                string modulus = key["n"].ToString();

                exponent = Convert.ToBase64String(Encoding.UTF8.GetBytes(exponent));
                modulus = Convert.ToBase64String(Encoding.UTF8.GetBytes(modulus));
                
                byte[] exponentBytes = Convert.FromBase64String(exponent);
                byte[] modulusBytes = Convert.FromBase64String(modulus);

                var rsaParams = new RSAParameters
                {
                    Exponent = exponentBytes,
                    Modulus = modulusBytes
                };

                using var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(rsaParams);
                _waasPublicKey = rsa.ToXmlString(false);
                PublicKeyFetched = true;
            }
            catch (Exception ex)
            {
                string error = $"Error fetching public key from WaaS API: {ex.Message}";
                Debug.LogError(error);
                throw new Exception(error);
            }
        }
        
        public string ValidateResponse(UnityWebRequest request)
        {
            byte[] results = request.downloadHandler.data;
            var responseJson = Encoding.UTF8.GetString(results);
            string contentDigest = request.GetResponseHeader("Content-Digest");
            if (string.IsNullOrWhiteSpace(contentDigest))
            {
                throw new Exception("No Content-Digest header found in response");
            }
            string digest = GetValue(contentDigest);
            
            byte[] expectedDigest = results.SHA256Hash();
            string expectedDigestBase64 = Convert.ToBase64String(expectedDigest);
            if (digest != expectedDigestBase64)
            {
                throw new Exception($"Content-Digest header does not match response content: {contentDigest} != {expectedDigest}");
            }
            
            string signatureItem = request.GetResponseHeader("Signature");
            if (string.IsNullOrWhiteSpace(signatureItem))
            {
                throw new Exception("No Signature header found in response");
            }
            string signature = GetValue(signatureItem);
            
            string signatureInput = request.GetResponseHeader("Signature-Input");
            if (string.IsNullOrWhiteSpace(signatureInput))
            {
                throw new Exception("No Signature-Input header found in response");
            }
            
            string signatureBase = $"\"content-digest\": {contentDigest}\n" +
                                   $"\"@signature-params\": {signatureInput}";
            
            VerifySignature(signatureBase, signature);

            return responseJson;
        }

        private void VerifySignature(string signatureBase, string signature)
        {
            try
            {
                byte[] signatureBytes = Convert.FromBase64String(signature);
                byte[] dataBytes = Encoding.UTF8.GetBytes(signatureBase);

                using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(_waasPublicKey);

                bool valid = rsa.VerifyData(dataBytes, CryptoConfig.MapNameToOID("SHA256"), signatureBytes);
                if (!valid)
                {
                    throw new Exception("Signature is invalid");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error verifying signature: {ex.Message}");
            }
        }

        private string GetValue(string headerItem)
        {
            int firstColon = headerItem.IndexOf(':');
            int lastColon = headerItem.LastIndexOf(':');
            if (firstColon == lastColon)
            {
                return null;
            }
            
            return headerItem.Substring(firstColon + 1, lastColon - firstColon - 1).Trim();
        }
    }
}