using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Envelope;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using UnityEngine;
using ConfigUpdate = Sequence.EcosystemWallet.KeyMachine.Models.ConfigUpdate;

namespace Sequence.EcosystemWallet
{
    internal class SignatureService
    {
        private readonly SessionsTopology _sessions;
        private readonly SignerService _signerService;

        public SignatureService(SessionSigner[] sessionSigners, SessionsTopology sessions)
        {
            _sessions = sessions;
            _signerService = new SignerService(sessionSigners, sessions);
        }

        public async Task<RawSignature> SignCalls(Chain chain, string imageHash, Envelope<Calls> envelope, ConfigUpdate[] configUpdates)
        {
            var signature = await SignSapient(chain, envelope);
            var sapientSignature = new SapientSignature
            {
                imageHash = imageHash,
                signature = signature
            };

            var signedEnvelope = envelope.ToSigned(sapientSignature);
            var rawSignature = SignatureHandler.EncodeSignature(signedEnvelope);
            
            rawSignature.suffix = configUpdates.Reverse().Select(u => 
                RawSignature.Decode(u.signature.HexStringToByteArray())).ToArray();

            return rawSignature;
        }
        
        private async Task<SignatureOfSapientSignerLeaf> SignSapient(Chain chain, Envelope<Calls> envelope)
        {
            var calls = envelope.payload.calls;
            if (calls.Length == 0)
                throw new Exception("calls is empty");

            var implicitSigners = new List<Address>();
            var explicitSigners = new List<Address>();

            var signers = await _signerService.FindSignersForCalls(chain, calls);
            
            var signatures = new SessionCallSignature[signers.Length];
            for (var i = 0; i < signers.Length; i++)
            {
                var signature = signers[i].SignCall(chain, calls[i], _sessions, envelope.payload.space, envelope.payload.nonce);
                signatures[i] = signature;
            }

            foreach (var signer in signers)
            {
                if (signer.IsExplicit)
                    explicitSigners.Add(signer.Address);
                else
                    implicitSigners.Add(signer.Address);
            }
            
            var sessionSignatures = SessionCallSignature.EncodeSignatures(
                signatures,
                _sessions, 
                explicitSigners.ToArray(), 
                implicitSigners.ToArray());
            
            return new SignatureOfSapientSignerLeaf
            {
                curType = SignatureOfSapientSignerLeaf.Type.sapient,
                address = new Address("0x06aa3a8F781F2be39b888Ac8a639c754aEe9dA29"), // Session manager address
                data = sessionSignatures
            };
        }
    }
}