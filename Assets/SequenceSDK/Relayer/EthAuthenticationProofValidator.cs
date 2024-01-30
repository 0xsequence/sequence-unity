using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Config;
using Sequence.Provider;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Relayer
{
    public static class EthAuthenticationProofValidator
    {
        public static async Task<bool> IsValidEthAuthProof(Chain chain, Address walletAddress, string ethAuthProof)
        {
            int chainId = (int)chain;
            IsValidEthAuthProofRequest requestObject = new IsValidEthAuthProofRequest("polygon", walletAddress, ethAuthProof);
            string payload = JsonConvert.SerializeObject(requestObject);

            using UnityWebRequest request = UnityWebRequest.Get("https://api.sequence.app/rpc/API/IsValidETHAuthProof");
            request.method = UnityWebRequest.kHttpVerbPOST;
            byte[] requestData = payload.ToByteArray();
            request.uploadHandler = new UploadHandlerRaw(requestData);
            request.uploadHandler.contentType = "application/json";
                
            SequenceConfig config = SequenceConfig.GetConfig();
            request.SetRequestHeader("X-Access-Key", config.BuilderAPIKey);
            request.SetRequestHeader("X-Access-Key", "YfeuczOMRyP7fpr1v7h8SvrCAAAAAAAAA"); // Todo: temporary access key while we wait for prod env deployment. Currently, we are using the staging env and we don't have a staging env for indexer that we can hit publicly
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            
            string curlRequest = $"curl -X POST -H \"X-Access-Key: {request.GetRequestHeader("X-Access-Key")}\" -H \"Content-Type: application/json\" -H \"Accept: application/json\" -d '{payload}' https://api.sequence.app/rpc/API/IsValidETHAuthProof";
            Debug.Log(curlRequest);
            
            await request.SendWebRequest();
            
            byte[] results = request.downloadHandler.data;
            request.Dispose();
            var responseJson = Encoding.UTF8.GetString(results);
            
            IsValidEthAuthProofResponse responseObject = JsonConvert.DeserializeObject<IsValidEthAuthProofResponse>(responseJson);
            return responseObject.isValid;
        }
    }

    [Serializable]
    public class IsValidEthAuthProofRequest
    {
        public IsValidEthAuthProofRequest(string chainId, string walletAddress, string ethAuthProofString)
        {
            this.chainId = chainId;
            this.walletAddress = walletAddress;
            this.ethAuthProofString = ethAuthProofString;
        }

        public string chainId;
        public string walletAddress;
        public string ethAuthProofString;
    }
    
    [Serializable]
    public class IsValidEthAuthProofResponse
    {
        public bool isValid;
    }
}