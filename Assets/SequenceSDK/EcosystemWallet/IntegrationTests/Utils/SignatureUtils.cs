using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public static class SignatureUtils
    {
        public static async Task<string> DoEncode(string input, List<string> signatures, bool noChainId,
            string? checkpointerData = null)
        {
            var config = ConfigUtils.FromJson(input);
            var allSignatures = signatures.Select(s =>
            {
                var values = s.Split(':');
                return new
                {
                    Address = new Address(values[0]),
                    Type = values[1],
                    Values = values.Skip(2).ToArray()
                };
            }).ToList();

            var fullTopology = Signature.FillLeaves(config.topology, leaf =>
            {
                if (leaf.isSignerLeaf)
                {
                    var signerLeaf = leaf as SignerLeaf;
                    var candidate = allSignatures.FirstOrDefault(s => s.Address == signerLeaf.address);
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
                                r = new BigInteger(candidate.Values[0].HexStringToByteArray(32)),
                                s = new BigInteger(candidate.Values[1].HexStringToByteArray(32)),
                                // TODO: yParity = OxSignature.VToYParity(int.Parse(candidate.Values[2])),
                            };
                        case "hash":
                            return new SignatureOfSignerLeafHash
                            {
                                r = new BigInteger(candidate.Values[0].HexStringToByteArray(32)),
                                s = new BigInteger(candidate.Values[1].HexStringToByteArray(32)),
                                // TODO: yParity = OxSignature.VToYParity(int.Parse(candidate.Values[2])),
                            };
                        case "sapient":
                        case "sapient_compact":
                            throw new Exception($"Incorrect type for leaf");
                        default:
                            throw new Exception($"Unsupported signature type: {candidate.Type}");
                    }
                }

                if (leaf.isSapientSignerLeaf)
                {
                    var signerLeaf = leaf as SapientSignerLeaf;
                    var candidate = allSignatures.FirstOrDefault(s => s.Address == signerLeaf.address);
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

            var encoded = new byte[] {};
            
            // TODO: 
            /*var encoded = Signature.EncodeSignature(new SignatureEncodingInput
            {
                NoChainId = noChainId,
                Configuration = new Configuration
                {
                    topology = fullTopology,
                },
                CheckpointerData = checkpointerData != null ? checkpointerData.ToByteArray() : null
            });

            return Hex.FromBytes(encoded);*/
            return encoded.ByteArrayToHexStringWithPrefix();
        }

        public static async Task<string> DoConcat(List<string> signatures)
        {
            if (signatures.Count == 0)
                throw new Exception("No signatures provided");

            var decoded = signatures
                // TODO: .Select(s => Signature.DecodeSignature(Bytes.FromHex(s)))
                .ToList();

            var encoded = new byte[] {};
            
            // TODO: 
            /*encoded = Signature.EncodeSignature(new SignatureEncodingInput
            {
                // Copy the base structure
                NoChainId = decoded[0].NoChainId,
                Configuration = decoded[0].Configuration,
                CheckpointerData = decoded[0].CheckpointerData,
                Suffix = decoded.Skip(1).ToList()
            });*/

            return encoded.ByteArrayToHexStringWithPrefix();
        }

        public static async Task<string> DoDecode(string signature)
        {
            // TODO: 
            /*var bytes = Bytes.FromHex(signature);
            var decoded = Signature.DecodeSignature(bytes);
            return Signature.RawSignatureToJson(decoded);*/
            return string.Empty;
        }
    }
}