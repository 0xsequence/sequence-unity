using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Envelope;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet
{
    public class GuardSigner
    {
        private readonly Address _guardSigner = ExtensionsFactory.Current.Guard;
        private readonly GuardService _service;
        
        public GuardSigner(GuardConfig config)
        {
            _service = new GuardService(config.url);
        }
        
        public async Task<Signature> SignEnvelope(SignedEnvelope<Calls> envelope)
        {
            var unparentedPayload = new Parented(Array.Empty<Address>(), envelope.payload);
            
            var payload = ToGuardPayload(envelope.wallet, envelope.chainId, unparentedPayload);
            
            var signatures = envelope.signatures;
            var guardSignatures = new GuardSignatureArgs[signatures.Length];
            for (var i = 0; i < signatures.Length; i++)
            {
                guardSignatures[i] = ToGuardSignature(signatures[i]);
                Debug.Log($"Guard sig {JsonConvert.SerializeObject(guardSignatures[i])}");
            }

            Debug.Log($"Nethereum TypedData Raw {Encoding.UTF8.GetString(payload.Message)}");
            Debug.Log($"Nethereum TypedData Hashed {payload.Digest.ByteArrayToHexStringWithPrefix()}");

            var rsy = await SignPayload(envelope.wallet, envelope.chainId, 
                payload.Digest, payload.Message, guardSignatures);
            
            return new Signature
            {
                address = _guardSigner,
                signature = new SignatureOfSignerLeafHash
                {
                    rsy = rsy
                }
            };
        }

        private async Task<RSY> SignPayload(Address wallet, BigInteger chainId, 
            byte[] digest, byte[] message, GuardSignatureArgs[] signatures)
        {
            var args = new SignWithArgs
            {
                signer = _guardSigner,
                request = new SignRequest
                {
                    chainId = chainId,
                    msg = digest.ByteArrayToHexStringWithPrefix(),
                    wallet = wallet,
                    payloadType = GuardPayloadType.Calls,
                    payloadData = message.ByteArrayToHexStringWithPrefix(),
                    signatures = signatures
                }
            };
            
            var response = await _service.SignWith(args);
            return RSY.UnpackFrom65(response.sig.HexStringToByteArray());
        }

        private (byte[] Message, byte[] Digest) ToGuardPayload(Address wallet, BigInteger chainId, Parented payload)
        {
            var typedData = new TypedDataToSign(wallet, chainId, payload);
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