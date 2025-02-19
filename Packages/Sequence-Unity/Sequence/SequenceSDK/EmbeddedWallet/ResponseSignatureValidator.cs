using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.EmbeddedWallet
{
    public class ResponseSignatureValidator
    {
        private RSAParameters _waasPublicKey;
        public bool PublicKeyFetched { get; private set; }
#if SEQUENCE_DEV_WAAS || SEQUENCE_DEV
        private const string _jwks = "{\"keys\":[{\"alg\":\"RS256\",\"e\":\"AQAB\",\"kid\":\"9LkLZyHdNq1N2aeHMlC5jw\",\"kty\":\"RSA\",\"n\":\"qllUB_ERsOjbKx4SirGow4XDov05lQyhiF7Duo4sPkH9CwMN11OqhLuIqeIXPq0rPNIXGP99A7riXTcpRNk-5ZNL29zs-Xjj3idp7nZQZLIU1CBQErTcbxbwUYp8Q46k7lJXVlMmwoLQvQAgH8BZLuSe-Xk1tye0mDC-bHvmrMfqm2zmuWeDnZercU3Jg2iYwyPrjKWx7YSBSMTXTKPGndws4m3s3XIEpI2alLcLLWsPQk2UjIlux6I7vLwvjM_BgjFhYHqgg1tgZUPn_Xxt4wvhobF8UIacRVmGcuyYBnhRxKnBQhEClGSBVtnFYYBSvRjTgliOwf3DhFoXdnmyPQ\",\"use\":\"sig\"}]}";
#else        
        private const string _jwks = "{\"keys\":[{\"alg\":\"RS256\",\"e\":\"AQAB\",\"kid\":\"nWh-_3nQ1lnhhI1ZSQTQmw\",\"kty\":\"RSA\",\"n\":\"pECaEq2k0k22J9e7hFLAFmKbzPLlWToUJJmFeWAdEiU4zpW17EUEOyfjRzjgBewc7KFJQEblC3eTD7Vc5bh9-rafPEj8LaKyZzzS5Y9ZATXhlMo5Pnlar3BrTm48XcnT6HnLsvDeJHUVbrYd1JyE1kqeTjUKWvgKX4mgIJiuYhpdzbOC22cPaWb1dYCVhArDVAPHGqaEwRjX7JneETdY5hLJ6JhsAws706W7fwfNKddPQo2mY95S9q8HFxMr5EaXEMmhwxk8nT5k-Ouar2dobMXRMmQiEZSt9fJaGKlK7KWJSnbPOVa2cZud1evs1Rz2SdCSA2bhuZ6NnZCxkqnagw\",\"use\":\"sig\"}]}";
#endif  
      
        public ResponseSignatureValidator()
        {
            LoadPublicKey();
        }

        private void LoadPublicKey()
        {
            try
            {
                var json = JObject.Parse(_jwks);
                var key = json["keys"][0];

                string exponent = key["e"].ToString();
                string modulus = key["n"].ToString();

                byte[] exponentBytes = Base64UrlDecode(exponent);
                byte[] modulusBytes = Base64UrlDecode(modulus);

                _waasPublicKey = new RSAParameters
                {
                    Exponent = exponentBytes,
                    Modulus = modulusBytes
                };
                PublicKeyFetched = true;
            }
            catch (Exception ex)
            {
                string error = $"Error loading public key from JWKS: {ex.Message}";
                Debug.LogError(error);
                throw new Exception(error);
            }
        }
        
        private byte[] Base64UrlDecode(string input)
        {
            string output = input.Replace('-', '+').Replace('_', '/');
            switch (output.Length % 4)
            {
                case 2: output += "=="; break;
                case 3: output += "="; break;
            }
            return Convert.FromBase64String(output);
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
                throw new Exception($"Content-Digest header does not match response content: {contentDigest} != {expectedDigestBase64}");
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
            string signatureInputValue = signatureInput.Replace("sig=", "");
            
            string signatureBase = $"\"content-digest\": {contentDigest}\n" +
                                   $"\"@signature-params\": {signatureInputValue}";
            
            VerifySignature(signatureBase, signature);

            return responseJson;
        }

        private void VerifySignature(string signatureBase, string signature)
        {
            try
            {
                byte[] signatureBytes = Convert.FromBase64String(signature);
                byte[] dataBytes = Encoding.UTF8.GetBytes(signatureBase);
                
                using var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(_waasPublicKey);

                bool valid = rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
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
