using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    public class EOAWalletLinker
    {
        private const string ApiUrl = "https://api.sequence.app/rpc/API/";
        private const string NonceGenerationLink = ApiUrl + "GenerateWaaSVerificationURL";
        private const string VerificationUrl = "https://demo-waas-wallet-link.pages.dev/";
        
        private IWallet _wallet;
        private Chain _chain;

        public EOAWalletLinker(IWallet wallet, Chain chain)
        {
            _wallet = wallet;
            _chain = chain;
        }

        public async Task<LinkedWalletData[]> GetLinkedWallets()
        {
            var parentWalletAddress = _wallet.GetWalletAddress();
            var messageToSign = $"parent wallet with address {parentWalletAddress}";
            var signature = await _wallet.SignMessage(_chain, messageToSign);

            try
            {
                var client = new HttpClient(ApiUrl);
                var response = await client.SendRequest<LinkedWalletsRequestData, LinkedWalletsResponseData>(
                    "GetLinkedWallets", new LinkedWalletsRequestData
                    {
                        signatureChainId = _chain.GetChainId(),
                        parentWalletAddress = parentWalletAddress,
                        parentWalletMessage = messageToSign,
                        parentWalletSignature = signature
                    });
                
                return response.linkedWallets;
            }
            catch (Exception e)
            {
                LogHandler.Error(e.Message);
                return Array.Empty<LinkedWalletData>();
            }
        }

        public async Task<bool> UnlinkWallet(string walletAddress)
        {
            var parentWalletAddress = _wallet.GetWalletAddress();
            var messageToSign = $"parent wallet with address {parentWalletAddress}";
            var signature = await _wallet.SignMessage(_chain, messageToSign);
            
            try
            {
                var client = new HttpClient(ApiUrl);
                await client.SendRequest<LinkedWalletsRequestData, LinkedWalletsResponseData>(
                    "RemoveLinkedWallet", new LinkedWalletsRequestData
                    {
                        signatureChainId = _chain.GetChainId(),
                        parentWalletAddress = parentWalletAddress,
                        parentWalletMessage = messageToSign,
                        parentWalletSignature = signature,
                        linkedWalletAddress = walletAddress
                    });
                
                return true;
            }
            catch (Exception e)
            {
                LogHandler.Error(e.Message);
                return false;
            }
        }

        public async Task<string> GenerateEoaWalletLink()
        {
            try
            {
                var client = new HttpClient(NonceGenerationLink);
                NonceResponseData nonceResponse =
                    await client.SendRequest<NonceRequestData, NonceResponseData>("",
                        new NonceRequestData(_wallet.GetWalletAddress()),
                        new Dictionary<string, string>()
                        {
                            { "X-Access-Key", null }
                        });
                IntentResponseSessionAuthProof proof = await _wallet.GetSessionAuthProof(_chain, nonceResponse.nonce);
                if (proof == null)
                {
                    throw new Exception("Received null session auth proof");
                }
                string eoaWalletLink = $"{VerificationUrl}?nonce={nonceResponse.nonce}&signature={proof.signature}&sessionId={proof.sessionId}&chainId={_chain.GetChainId()}";
                return eoaWalletLink;
            }
            catch (System.Exception e)
            {
                LogHandler.Error($"Failed to generate EOA Wallet Link: {e}");
                return null;
            }
        }
        
        public async Task OpenEoaWalletLink()
        {
            var link = await GenerateEoaWalletLink();
            AppEnvironment.OpenUrl(link);
        }
    }
    
    internal class LinkedWalletsRequestData
    {
        public string signatureChainId;
        public string parentWalletAddress;
        public string parentWalletMessage;
        public string parentWalletSignature;
        public string linkedWalletAddress;
    }
    
    internal class LinkedWalletsResponseData
    {
        public LinkedWalletData[] linkedWallets;
    }

    public class LinkedWalletData
    {
        public int id;
        public string walletType;
        public string walletAddress;
        public string linkedWalletAddress;
        public string createdAt;
    }

    internal class NonceRequestData
    {
        public string walletAddress;
        
        public NonceRequestData(string walletAddress)
        {
            this.walletAddress = walletAddress;
        }
    }

    internal class NonceResponseData
    {
        public string nonce;
        public string verificationUrl;

        public NonceResponseData(string nonce, string verificationUrl)
        {
            this.nonce = nonce;
            this.verificationUrl = verificationUrl;
        }
    }
}