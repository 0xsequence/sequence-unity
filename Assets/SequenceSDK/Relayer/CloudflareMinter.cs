using System;
using System.IO;
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
        private const string _mintEndpoint = "https://c801-70-26-76-48.ngrok-free.app/";
        
        private EthAuthenticationProof _ethAuthenticationProof;
        
        public CloudflareMinter(EthAuthenticationProof ethAuthenticationProof)
        {
            _ethAuthenticationProof = ethAuthenticationProof;
        }
        
        public event Action<string> OnMintTokenSuccess;
        public event Action<string> OnMintTokenFailed;
        public async Task MintToken(string tokenId, uint amount = 1)
        {
            string proof = await _ethAuthenticationProof.GenerateProof();
            
            MintTokenRequest mintTokenRequest = new MintTokenRequest()
            {
                proof = proof,
                address = _ethAuthenticationProof.GetSigningAddress(),
                tokenId = ulong.Parse(tokenId),
                amount = amount
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
                    return;
                }
                OnMintTokenFailed?.Invoke("Error minting using cloudflare: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl request: " + curlRequest);
            }
            catch (Exception e)
            {
                OnMintTokenFailed?.Invoke("Error minting using cloudflare: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.downloadHandler.data) + "\nCurl request: " + curlRequest);
            }
        }
    }

    [Serializable]
    public class MintTokenRequest
    {
        public string proof;
        public string address;
        public ulong tokenId;
        public uint amount;
    }
}