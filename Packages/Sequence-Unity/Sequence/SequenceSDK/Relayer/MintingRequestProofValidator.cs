using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Config;
using Sequence.Provider;
using Sequence.Utils;
using Sequence.WaaS;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Relayer
{
    public static class MintingRequestProofValidator
    {
        public static async Task<bool> IsValidMintingRequestProof(Chain chain, MintingRequestProof proof)
        {
            WaaSWallet wallet = new WaaSWallet(proof.SigningAddress, null, null);

            try
            {
                IsValidMessageSignatureReturn isValid =
                    await wallet.IsValidMessageSignature(chain, proof.Proof, proof.SignedProof);
                return isValid.isValid;
            }
            catch (Exception e)
            {
                throw new Exception("Error validating Minting Request Proof: " + e.Message);
            }
        }
    }
}