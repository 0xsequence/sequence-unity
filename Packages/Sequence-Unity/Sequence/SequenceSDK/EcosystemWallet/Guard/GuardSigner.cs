using System;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Envelope;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet
{
    public class GuardSigner
    {
        private readonly Address _address;
        private readonly GuardConfig _config;
        private readonly GuardService _service;
        
        public GuardSigner(Address address, GuardConfig config)
        {
            Debug.Log($"Guard config {JsonConvert.SerializeObject(config)}");
            _address = address;
            _config = config;
            _service = new GuardService(config.url);
        }
        
        public async Task<Signature> SignEnvelope(SignedEnvelope<Calls> envelope)
        {
            var unparentedPayload = new Parented(Array.Empty<Address>(), envelope.payload);
            
            var payloadType = ToGuardPayloadType();
            var payload = ToGuardPayload(envelope.chainId, unparentedPayload);
            
            var signatures = envelope.signatures;
            var guardSignatures = new GuardSignatureArgs[signatures.Length];
            for (var i = 0; i < signatures.Length; i++)
                guardSignatures[i] = ToGuardSignature(signatures[i]);

            var rsy = await SignPayload(envelope.wallet, envelope.chainId, payloadType, 
                payload.Digest, payload.Message, guardSignatures);
            
            return new Signature
            {
                address = _address,
                signature = new SignatureOfSignerLeafHash
                {
                    rsy = rsy
                }
            };
        }

        private async Task<RSY> SignPayload(Address wallet, BigInteger chainId, GuardPayloadType payloadType, 
            byte[] digest, byte[] message, GuardSignatureArgs[] signatures)
        {
            var response = await _service.SignWith(new SignWithArgs
            {
                signer = _address,
                request = new SignRequest
                {
                    chainId = chainId,
                    msg = digest.ByteArrayToHexStringWithPrefix(),
                    wallet = wallet,
                    payloadType = payloadType,
                    payloadData = message.ByteArrayToHexStringWithPrefix(),
                    signatures = signatures
                }
            });

            return RSY.FromString(response.sig);
        }

        private GuardPayloadType ToGuardPayloadType()
        {
            return GuardPayloadType.Calls;
        }

        private (byte[] Message, byte[] Digest) ToGuardPayload(BigInteger chainId, Parented payload)
        {
            var isImplicit = false;
            if (isImplicit)
            {
                return (
                    Message: null, 
                    Digest: null);
            }

            var typedData = new TypedDataToSign(_address, chainId, payload);
            return (
                Message: JsonConvert.SerializeObject(typedData).ToByteArray(), 
                Digest: typedData.GetSignPayload());
        }

        private GuardSignatureArgs ToGuardSignature(EnvelopeSignature signature)
        {
            if (signature is SapientSignature sapientSignature)
            {
                return new GuardSignatureArgs
                {
                    type = GuardSignatureType.Sapient,
                    address = sapientSignature.signature.address,
                    imageHash = sapientSignature.imageHash,
                    data = sapientSignature.signature.data.ByteArrayToHexStringWithPrefix()
                };
            }

            if (signature is Signature { signature: SignatureOfSignerLeafErc1271 erc1271 })
            {
                return new GuardSignatureArgs
                {
                    type = GuardSignatureType.Erc1271,
                    address = erc1271.address,
                    data = erc1271.data.ByteArrayToHexStringWithPrefix()
                };
            }

            if (signature is Signature { signature: SignatureOfSignerLeafEthSign ethSign } ethSignSig)
            {
                return new GuardSignatureArgs
                {
                    type = GuardSignatureType.EthSign,
                    address = ethSignSig.address,
                    data = ethSign.rsy.Pack().ByteArrayToHexStringWithPrefix()
                }; 
            }
            
            if (signature is Signature { signature: SignatureOfSignerLeafEthSign hash } hashSig)
            {
                return new GuardSignatureArgs
                {
                    type = GuardSignatureType.Hash,
                    address = hashSig.address,
                    data = hash.rsy.Pack().ByteArrayToHexStringWithPrefix()
                }; 
            }
            
            throw new System.Exception("Unknown signature type");
        }
    }
}