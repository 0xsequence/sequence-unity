using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Envelope;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using ConfigUpdate = Sequence.EcosystemWallet.KeyMachine.Models.ConfigUpdate;

namespace Sequence.EcosystemWallet
{
    internal class SignatureService
    {
        private readonly SessionsTopology _sessions;
        private readonly SignerService _signerService;
        private SessionSigner[] _currentSigners;

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
            
            var guardConfig = new GuardStorage().GetConfig(envelope.wallet);
            if (guardConfig != null)
            {
                Attestation attestation = null;
                foreach (var signer in _currentSigners)
                {
                    if (!signer.IsExplicit)
                        attestation = signer.Attestation;
                }
                
                var guardSigner = new GuardSigner(guardConfig, attestation);
                var guardSignature = await guardSigner.SignEnvelope(signedEnvelope);
                signedEnvelope.signatures = signedEnvelope.signatures.AddToArray(guardSignature);   
            }
            
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

            _currentSigners = await _signerService.FindSignersForCalls(chain, calls);
            
            var signatures = new SessionCallSignature[_currentSigners.Length];
            for (var i = 0; i < _currentSigners.Length; i++)
            {
                var signature = _currentSigners[i].SignCall(chain, calls[i], _sessions, envelope.payload.space, envelope.payload.nonce);
                signatures[i] = signature;
            }

            foreach (var signer in _currentSigners)
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
                address = ExtensionsFactory.Current.Sessions,
                data = sessionSignatures
            };
        }
    }
}