using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Config;
using Sequence.Provider;
using Sequence.Utils;
using Sequence.EmbeddedWallet;

namespace Sequence.Relayer
{
    public static class MintingRequestProofValidator
    {
        public static async Task<bool> IsValidMintingRequestProof(Chain chain, MintingRequestProof proof)
        {
            SequenceWallet wallet = new SequenceWallet(proof.SigningAddress, null, null, "");

            try
            {
                IsValidMessageSignatureReturn result =
                    await wallet.IsValidMessageSignature(chain, proof.Proof, proof.SignedProof);
                return result.isValid;
            }
            catch (Exception e)
            {
                throw new Exception("Error validating Minting Request Proof: " + e.Message);
            }
        }
    }
}