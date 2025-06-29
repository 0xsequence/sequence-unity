using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class SignatureTests
    {
        public Task<string> SignatureEncode(Dictionary<string, object> parameters)
        {
            var input = parameters["input"].ToString();
            var signatures = parameters["signatures"].ToString();
            var chainId = !parameters.TryGetValue("chainId", out var chainIdValue) || (bool)chainIdValue;

            return Task.FromResult(EncodeSignatureFromInput(input, signatures, !chainId));
        }

        public Task<string> SignatureDecode(Dictionary<string, object> parameters)
        {
            var encodedSignature = parameters["signature"].ToString().HexStringToByteArray();
            var signature = RawSignature.Decode(encodedSignature);
            
            return Task.FromResult(JsonConvert.SerializeObject(signature));
        }
        
        public Task<string> SignatureConcat(Dictionary<string, object> parameters)
        {
            var signatures = parameters.GetArray<string>("signatures");
            var decoded = signatures.Select(signature => 
                    RawSignature.Decode(signature.HexStringToByteArray())).ToArray();

            var parentSignature = decoded[0];
            parentSignature.suffix = decoded.Slice(1);
            var encoded = parentSignature.Encode();
            
            return Task.FromResult(encoded.ByteArrayToHexStringWithPrefix());
        }

        private static string EncodeSignatureFromInput(string input, string signatures, bool noChainId, byte[] checkpointerData = null)
        {
            var parts = string.IsNullOrEmpty(signatures) ? 
                Array.Empty<string>() : 
                signatures.Split(' ');
            
            var config = Primitives.Config.FromJson(input);
            
            var allSignatures = parts.Select(s =>
            {
                var values = s.Split(':');
                
                return new
                {
                    Address = new Address(values[0]),
                    Type = values[1],
                    Values = values.Skip(2).ToArray()
                };
            }).ToList();

            var fullTopology = SignatureHandler.FillLeaves(config.topology, leaf =>
            {
                if (leaf is SignerLeaf signerLeaf)
                {
                    var candidate = allSignatures.FirstOrDefault(s => s.Address.Equals(signerLeaf.address));
                    if (candidate == null) 
                        return null;
                    
                    switch (candidate.Type)
                    {
                        case "erc1271":
                            return new SignatureOfSignerLeafErc1271
                            {
                                address = candidate.Address,
                                data = candidate.Values[0].HexStringToByteArray()
                            };
                        case "eth_sign":
                            return new SignatureOfSignerLeafEthSign
                            {
                                rsy = new RSY
                                {
                                    r = candidate.Values[0].HexStringToBigInteger(),
                                    s = candidate.Values[1].HexStringToBigInteger(),
                                    yParity = RSY.VToYParity(int.Parse(candidate.Values[2]))
                                }
                            };
                        case "hash":
                            return new SignatureOfSignerLeafHash
                            {
                                rsy = new RSY
                                {
                                    r = candidate.Values[0].HexStringToBigInteger(),
                                    s = candidate.Values[1].HexStringToBigInteger(),
                                    yParity = RSY.VToYParity(int.Parse(candidate.Values[2]))
                                }
                            };
                        case "sapient":
                            return new SignatureOfSapientSignerLeaf
                            {
                                curType = SignatureOfSapientSignerLeaf.Type.sapient,
                                address = candidate.Address,
                                data = candidate.Values[0].HexStringToByteArray()
                            };
                        case "sapient_compact":
                            return new SignatureOfSapientSignerLeaf
                            {
                                curType = SignatureOfSapientSignerLeaf.Type.sapient_compact,
                                address = candidate.Address,
                                data = candidate.Values[0].HexStringToByteArray()
                            };
                        default:
                            throw new Exception($"Unsupported signature type: {candidate.Type}");
                    }
                }

                if (leaf is SapientSignerLeaf sapientSignerLeaf)
                {
                    var candidate = allSignatures.FirstOrDefault(s => s.Address.Equals(sapientSignerLeaf.address));
                    if (candidate == null) 
                        return null;

                    switch (candidate.Type)
                    {
                        case "sapient":
                        case "sapient_compact":
                            return new SignatureOfSapientSignerLeaf
                            {
                                address = candidate.Address,
                                data = candidate.Values[0].HexStringToByteArray(),
                            };
                        case "eth_sign":
                        case "hash":
                        case "erc1271":
                            throw new Exception($"Incorrect type for leaf");
                        default:
                            throw new Exception($"Unsupported signature type: {candidate.Type}");
                    }
                }

                return null;
            });

            var rawSignature = new RawSignature
            {
                noChainId = noChainId,
                configuration = new Primitives.Config
                {
                    threshold = config.threshold,
                    checkpoint = config.checkpoint,
                    checkpointer = config.checkpointer,
                    topology = fullTopology
                },
                checkpointerData = checkpointerData
            };

            return rawSignature.Encode().ByteArrayToHexStringWithPrefix();
        }
    }
}