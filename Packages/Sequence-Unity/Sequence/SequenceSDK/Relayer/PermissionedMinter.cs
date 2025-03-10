using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EmbeddedWallet;
using Sequence.Utils;

namespace Sequence.Relayer
{
    public class PermissionedMinter : IMinter
    {
        private string _mintEndpoint;
        
        protected MintingRequestProver _mintingRequestProver;

        protected Address _contractAddress;
        
        public PermissionedMinter(MintingRequestProver mintingRequestProver, string mintEndpoint, string contractAddress)
        {
            _mintingRequestProver = mintingRequestProver;
            _mintEndpoint = mintEndpoint.AppendTrailingSlashIfNeeded();
            _contractAddress = new Address(contractAddress);
        }
        
        public event Action<string> OnMintTokenSuccess;
        public event Action<string> OnMintTokenFailed;

        public virtual async Task<string> BuildMintTokenRequestJson(string tokenId, uint amount = 1)
        {
            MintingRequestProof requestProof =
                await _mintingRequestProver.GenerateProof(_contractAddress, tokenId, amount);

            MintTokenRequest mintTokenRequest = new MintTokenRequest(requestProof);

            string mintTokenRequestJson = JsonConvert.SerializeObject(mintTokenRequest);
            return mintTokenRequestJson;
        }

        public async Task<string> MintToken(string tokenId, uint amount = 1)
        {
            string mintTokenRequestJson = await BuildMintTokenRequestJson(tokenId, amount);
            string transactionHash = await SendMintTokenRequest(mintTokenRequestJson);
            return transactionHash;
        }
        
        private async Task<string> SendMintTokenRequest(string mintTokenRequestJson)
        {
            using IWebRequest request = WebRequestBuilder.Post(_mintEndpoint, string.Empty);
            byte[] requestData = mintTokenRequestJson.ToByteArray();
            request.SetRequestData(requestData);
            request.SetRequestHeader("Content-Type", "application/json");
            
            string curlRequest = $"curl -X POST -H \"Content-Type: application/json\" -d '{mintTokenRequestJson}' {_mintEndpoint}";

            try
            {
                await request.Send();

                string transactionHash = request.Text;
                request.Dispose();

                OnMintTokenSuccess?.Invoke(transactionHash);
            }
            catch (FileLoadException e)
            {
                if (request.Text != null) // Unity sometimes throws an exception here even though the response can be properly handled. I have no idea why...
                {
                    string transactionHash = request.Text;
                    request.Dispose();

                    if (!transactionHash.StartsWith("0x"))
                    {
                        OnMintTokenFailed?.Invoke("Error minting using permissioned minter: " + transactionHash + "\nCurl request: " + curlRequest);
                        return null;
                    }

                    OnMintTokenSuccess?.Invoke(transactionHash);
                    return transactionHash;
                }
                OnMintTokenFailed?.Invoke("Error minting using permissioned minter: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.Data) + "\nCurl request: " + curlRequest);
            }
            catch (Exception e)
            {
                OnMintTokenFailed?.Invoke("Error minting using permissioned minter: " + e.Message + " reason: " + Encoding.UTF8.GetString(request.Data) + "\nCurl request: " + curlRequest);
            }
            finally
            {
                request.Dispose();
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

        public MintTokenRequest(MintingRequestProof proof)
        {
            this.proof = proof.Proof;
            this.signedProof = proof.SignedProof;
            this.address = proof.SigningAddress;
        }
        
        [JsonConstructor]
        public MintTokenRequest(string proof, string signedProof, string address)
        {
            this.proof = proof;
            this.signedProof = signedProof;
            this.address = address;
        }
    }
}