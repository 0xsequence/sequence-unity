using System.Threading.Tasks;
using UnityEngine;

namespace Sequence.EmbeddedWallet
{
    public class EOAWalletLinker
    {
        private string _nonceGenerationLink;
        
        private IWallet _wallet;

        public EOAWalletLinker(IWallet wallet, string nonceGenerationLink)
        {
            _wallet = wallet;
            _nonceGenerationLink = nonceGenerationLink;
        }

        public async Task<string> GenerateEOAWalletLink(Chain chain)
        {
            IHttpClient client = new HttpClient(_nonceGenerationLink);
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