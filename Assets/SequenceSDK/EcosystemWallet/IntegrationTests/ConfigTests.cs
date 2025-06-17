using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class ConfigTests
    {
        public Task<string> ConfigNew(Dictionary<string, object> parameters)
        {
            var threshold = (string)parameters["threshold"];
            var checkpoint = (string)parameters["checkpoint"];
            var content = (string)parameters["content"];
            
            var checkpointer = parameters.TryGetValue("checkpointer", out var checkpointerValue) && 
                               checkpointerValue != null ? new Address(checkpointerValue as string) : null;
            
            var config = new Primitives.Config
            {
                threshold = BigInteger.Parse(threshold),
                checkpoint = checkpoint.HexStringToBigInteger(),
                topology = Topology.FromLeaves(ParseContentToLeafs(content)),
                checkpointer = checkpointer
            };

            return Task.FromResult(config.ToJson());
        }
        
        public Task<string> ConfigEncode(Dictionary<string, object> parameters)
        {
            throw new System.NotImplementedException();
        }
        
        public Task<string> ConfigImageHash(Dictionary<string, object> parameters)
        {
            var input = parameters["input"].ToString();
            var config = ConfigUtils.FromJson(input);
            var imageHash = config.HashConfiguration().ByteArrayToHexStringWithPrefix();
            return Task.FromResult(imageHash);
        }

        private List<Leaf> ParseContentToLeafs(string elements)
        {
            var leaves = new List<Leaf>();

            while (!string.IsNullOrWhiteSpace(elements))
            {
                string firstElement = elements.Split(' ')[0];
                string firstElementType = firstElement.Split(':')[0];

                if (firstElementType == "signer")
                {
                    var parts = firstElement.Split(':');
                    string address = parts[1];
                    BigInteger weight = BigInteger.Parse(parts[2]);

                    leaves.Add(new SignerLeaf
                    {
                        address = new  Address(address),
                        weight = weight
                    });

                    elements = elements.Substring(firstElement.Length).TrimStart();
                }
                else if (firstElementType == "subdigest")
                {
                    var parts = firstElement.Split(':');
                    string digest = parts[1];

                    leaves.Add(new SubdigestLeaf
                    {
                        digest = digest.HexStringToByteArray()
                    });

                    elements = elements.Substring(firstElement.Length).TrimStart();
                }
                else if (firstElementType == "any-address-subdigest")
                {
                    var parts = firstElement.Split(':');
                    string digest = parts[1];

                    leaves.Add(new AnyAddressSubdigestLeaf
                    {
                        digest = digest.HexStringToByteArray()
                    });

                    elements = elements.Substring(firstElement.Length).TrimStart();
                }
                else if (firstElementType == "sapient")
                {
                    var parts = firstElement.Split(':');
                    string imageHash = parts[1];
                    string address = parts[2];
                    BigInteger weight = BigInteger.Parse(parts[3]);

                    if (!imageHash.StartsWith("0x") || imageHash.Length != 66)
                        throw new Exception($"Invalid image hash: {imageHash}");

                    leaves.Add(new SapientSignerLeaf
                    {
                        imageHash = imageHash,
                        address = new Address(address),
                        weight = weight
                    });

                    elements = elements.Substring(firstElement.Length).TrimStart();
                }
                else if (firstElementType == "nested")
                {
                    var parts = firstElement.Split(':');
                    BigInteger threshold = BigInteger.Parse(parts[1]);
                    BigInteger weight = BigInteger.Parse(parts[2]);

                    int start = elements.IndexOf('(');
                    int end = elements.IndexOf(')');
                    if (start == -1 || end == -1)
                        throw new Exception($"Missing ( ) for nested element: {elements}");

                    string inner = elements.Substring(start + 1, end - start - 1);

                    leaves.Add(new NestedLeaf
                    {
                        threshold = threshold,
                        weight = weight,
                        tree = Topology.FromLeaves(ParseContentToLeafs(inner))
                    });

                    elements = elements.Substring(end + 1).TrimStart();
                }
                else if (firstElementType == "node")
                {
                    var parts = firstElement.Split(':');
                    string hash = parts[1];

                    leaves.Add(new NodeLeaf
                    {
                        Value = hash.HexStringToByteArray()
                    });

                    elements = elements.Substring(firstElement.Length).TrimStart();
                }
                else
                {
                    throw new Exception($"Invalid element: {firstElement}");
                }
            }

            return leaves;
        }
    }
}