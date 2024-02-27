using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence;
using Sequence.Utils;
using Sequence.Wallet;

namespace Sequence.Relayer
{
    public class EthAuthenticationProof
    {
        private IProofSigner _proofSigner;
        private Chain _chain;
        
        public EthAuthenticationProof(IWallet eoaWallet, Chain chain)
        {
            _chain = chain;
            _proofSigner = new EOAProofSigner(eoaWallet, _chain);
        }
        
        public EthAuthenticationProof(Sequence.WaaS.IWallet waasWallet, Chain chain)
        {
            _chain = chain;
            _proofSigner = new WaaSProofSigner(waasWallet, _chain);
        }

        public async Task<string> GenerateProof()
        {
            ProofPayload proofPayload = new ProofPayload()
            {
                app = "Made with Sequence Unity SDK App",
                iat = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                exp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 300,
                v = "1",
                ogn = "Sequence Unity SDK"
            };
            string proof = JsonConvert.SerializeObject(proofPayload);
            string base64Proof = Convert.ToBase64String(proof.ToByteArray());
            string signedProof = await _proofSigner.SignProof(base64Proof);
            string ethAuthenticationProof = $"eth.{_proofSigner.SigningAddress()}.{base64Proof}.{signedProof}";
            return ethAuthenticationProof;
        }
        
        public string GetSigningAddress()
        {
            return _proofSigner.SigningAddress();
        }

        private interface IProofSigner
        {
            public Task<string> SignProof(string proof);
            public string SigningAddress();
        }
        
        private class EOAProofSigner : IProofSigner
        {
            private IWallet _eoaWallet;
            private Chain _chain;

            public EOAProofSigner(IWallet eoaWallet, Chain chain)
            {
                _eoaWallet = eoaWallet;
                _chain = chain;
            }
            
            public async Task<string> SignProof(string proof)
            {
                return await _eoaWallet.SignMessage(proof);
            }

            public string SigningAddress()
            {
                return _eoaWallet.GetAddress();
            }
        }
        
        private class WaaSProofSigner : IProofSigner
        {
            private Sequence.WaaS.IWallet _waasWallet;
            private Chain _chain;

            public WaaSProofSigner(Sequence.WaaS.IWallet waasWallet, Chain chain)
            {
                _waasWallet = waasWallet;
                _chain = chain;
            }
            
            public async Task<string> SignProof(string proof)
            {
                return await _waasWallet.SignMessage(_chain, proof);
            }
            
            public string SigningAddress()
            {
                return _waasWallet.GetWalletAddress();
            }
        }
    }
    
    [Serializable]
    public class ProofPayload
    {
        public string app;
        public uint iat;
        public uint exp;
        public string v;
        public string ogn;
    }
}