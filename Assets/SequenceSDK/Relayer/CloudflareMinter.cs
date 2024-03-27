using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Provider;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Relayer
{
    public class CloudflareMinter
    {
        private string _mintEndpoint;
        
        private MintingRequestProver _mintingRequestProver;

        private Address _contractAddress;
        
        public CloudflareMinter(MintingRequestProver mintingRequestProver, string mintEndpoint, string contractAddress)
        {
            _mintingRequestProver = mintingRequestProver;
            _mintEndpoint = mintEndpoint.AppendTrailingSlashIfNeeded();
            _contractAddress = new Address(contractAddress);
        }
        
        public event Action<string> OnMintTokenSuccess;
        public event Action<string> OnMintTokenFailed;
        public async Task<string> MintToken(string tokenId, uint amount = 1)
        {
            MintingRequestProof requestProof = await _mintingRequestProver.GenerateProof(_contractAddress, tokenId, amount);
            
            MintTokenRequest mintTokenRequest = new MintTokenRequest()
            {
                proof = requestProof.Proof,
                signedProof = requestProof.SignedProof,
                address = requestProof.SigningAddress,
            };
            
            string mintTokenRequestJson = JsonConvert.SerializeObject(mintTokenRequest);
            
            UnityWebRequest request = UnityWebRequest.Get(_mintEndpoint);
            request.method = UnityWebRequest.kHttpVerbPOST;
            byte[] requestData = mintTokenRequestJson.ToByteArray();
            request.uploadHandler = new UploadHandlerRaw(requestData);
            request.uploadHandler.contentType = "application/json";
            request.SetRequestHeader("Content-Type", "application/json");
            
            string curlRequest = $"curl -X POST -H \"Content-Type: application/json\" -d '{mintTokenRequestJson}' {_mintEndpoint}";

            try
            {
                await request.SendWebRequest();

                string transactionHash = request.downloadHandler.text;
                request.Dispose();

                OnMintTokenSuccess?.Invoke(transactionHash);
            }
            catch (FileLoadException e)
            {
                if (request.downloadHandler != null) // Unity sometimes throws an exception here even though the response can be properly handled. I have no idea why...
                {
                    string transactionHash = request.downloadHandler.text;
                    request.Dispose();

                    OnMintTokenSuccess?.Invoke(transactionHash);
                    return transactionHash;
                }
                OnMintTokenFailed?.Invoke("Error minting using cloudflare: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl request: " + curlRequest);
            }
            catch (Exception e)
            {
                OnMintTokenFailed?.Invoke("Error minting using cloudflare: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl request: " + curlRequest);
            }

            return null;
        }
    }

    [Serializable]
    public class MintTokenRequest
    {
        public string proof;
        public string signedProof;
        public string address;
    }
}