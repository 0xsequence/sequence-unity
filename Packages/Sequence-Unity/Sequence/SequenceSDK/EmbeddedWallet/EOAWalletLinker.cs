using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Sequence.EmbeddedWallet
{
    public class EOAWalletLinker
    {
        private string _nonceGenerationLink;
        
        private IWallet _wallet;

        private string _verificationUrl;// = "https://demo-waas-wallet-link.pages.dev/";

        public EOAWalletLinker(IWallet wallet, string nonceGenerationLink, string verificationUrl)
        {
            _wallet = wallet;
            _nonceGenerationLink = nonceGenerationLink;
            _verificationUrl = verificationUrl;
        }

        public async Task<string> GenerateEOAWalletLink(Chain chain)
        {
            IHttpClient client = new HttpClient(_nonceGenerationLink);
            try
            {
                NonceResponseData nonceResponse =
                    await client.SendRequest<NonceRequestData, NonceResponseData>("",
                        new NonceRequestData(_wallet.GetWalletAddress()),
                        new Dictionary<string, string>()
                        {
                            { "X-Access-Key", null }
                        });
                IntentResponseSessionAuthProof proof = await _wallet.GetSessionAuthProof(chain, nonceResponse.nonce);
                if (proof == null)
                {
                    throw new Exception("Received null session auth proof");
                }
                string eoaWalletLink = $"{_verificationUrl}?nonce={nonceResponse.nonce}&signature={proof.signature}&sessionId={proof.sessionId}&chainId={chain.GetChainId()}";
                return eoaWalletLink;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to generate EOA Wallet Link: {e}");
                return null;
            }
        }
        
        public async Task OpenEOAWalletLink(Chain chain)
        {
            string link = await GenerateEOAWalletLink(chain);
            Application.OpenURL(link);
        }
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