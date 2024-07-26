using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence;
using Sequence.Utils;
using Sequence.Wallet;

namespace Sequence.Relayer
{
    public class MintingRequestProver
    {
        private IProofSigner _proofSigner;
        private Chain _chain;
        
        public MintingRequestProver(IWallet eoaWallet, Chain chain)
        {
            _chain = chain;
            _proofSigner = new EOAProofSigner(eoaWallet, _chain);
        }
        
        public MintingRequestProver(Sequence.EmbeddedWallet.IWallet wallet, Chain chain)
        {
            _chain = chain;
            _proofSigner = new SequenceWalletProofSigner(wallet, _chain);
        }

        public async Task<MintingRequestProof> GenerateProof(string contractAddress, string tokenId, uint amount)
        {
            ProofPayload proofPayload = new ProofPayload()
            {
                app = "Made with Sequence Unity SDK App",
                iat = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                exp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 300,
                ogn = "Sequence Unity SDK",
                payload = new MintRequestPayload()
                {
                    contractAddress = contractAddress,
                    tokenId = tokenId,
                    amount = amount
                }
            };
            string proof = JsonConvert.SerializeObject(proofPayload);
            string signedProof = await _proofSigner.SignProof(proof);
            return new MintingRequestProof(proof, signedProof, new Address(_proofSigner.SigningAddress()));
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
        
        private class SequenceWalletProofSigner : IProofSigner
        {
            private Sequence.EmbeddedWallet.IWallet _wallet;
            private Chain _chain;

            public SequenceWalletProofSigner(Sequence.EmbeddedWallet.IWallet wallet, Chain chain)
            {
                _wallet = wallet;
                _chain = chain;
            }
            
            public async Task<string> SignProof(string proof)
            {
                return await _wallet.SignMessage(_chain, proof);
            }
            
            public string SigningAddress()
            {
                return _wallet.GetWalletAddress();
            }
        }
    }

    public class MintingRequestProof
    {
        public string Proof;
        public string SignedProof;
        public Address SigningAddress;

        public MintingRequestProof(string proof, string signedProof, Address signingAddress)
        {
            Proof = proof;
            SignedProof = signedProof;
            SigningAddress = signingAddress;
        }

        public MintingRequestProof(MintTokenRequest request)
        {
            Proof = request.proof;
            SignedProof = request.signedProof;
            SigningAddress = new Address(request.address);
        }
    }
    
    [Serializable]
    public class ProofPayload
    {
        public string app;
        public uint exp;
        public uint iat;
        public string ogn;
        public MintRequestPayload payload;
    }

    [Serializable]
    public class MintRequestPayload
    {
        public string contractAddress;
        public string tokenId;
        public uint amount;
    }
}