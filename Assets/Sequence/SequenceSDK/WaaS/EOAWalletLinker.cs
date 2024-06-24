using System.Threading.Tasks;
using UnityEngine;

namespace Sequence.WaaS
{
    public class EOAWalletLinker
    {
        private static string NonceGenerationLink =
            "https://demo-waas-wallet-link-server.tpin.workers.dev/generateNonce";
        
        private IWallet _wallet;

        public EOAWalletLinker(IWallet wallet)
        {
            _wallet = wallet;
        }

        public async Task<string> GenerateEOAWalletLink(Chain chain)
        {
            IHttpClient client = new HttpClient(NonceGenerationLink);
            NonceResponseData nonceResponse =
                await client.SendRequest<NonceRequestData, NonceResponseData>("",
                    new NonceRequestData(_wallet.GetWalletAddress()));
            IntentResponseSessionAuthProof proof = await _wallet.GetSessionAuthProof(chain, nonceResponse.nonce);
            string eoaWalletLink = $"{nonceResponse.verificationUrl}?nonce={nonceResponse.nonce}&signature={proof.signature}&sessionId={proof.sessionId}&chainId={chain.GetChainId()}";
            return eoaWalletLink;
        }
        
        public async Task OpenEOAWalletLink(Chain chain)
        {
            string link = await GenerateEOAWalletLink(chain);
            Application.OpenURL(link);
        }
        
        public static void InjectNonceGenerationLink(string link)
        {
            NonceGenerationLink = link;
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