using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            var content = ((string)parameters["content"]).Split(' ');
            var checkpointer = (string)parameters["checkpointer"];
            
            var config = new Primitives.Config
            {
                threshold = BigInteger.Parse(threshold),
                checkpoint = checkpoint.HexStringToBigInteger(),
                topology = Topology.FromLeaves(ParseContentToLeafs(content)),
                checkpointer = new Address(checkpointer)
            };

            return Task.FromResult(config.ToJson());
        }
        
        public async Task<string> ConfigEncode(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException("Not implemented");
        }
        
        public Task<string> ConfigImageHash(Dictionary<string, object> parameters)
        {
            var input = (JsonToken)parameters["input"];
            var inputJson = JsonConvert.SerializeObject(input);

            var config = Primitives.Config.FromJson(inputJson);
            var imageHash = config.HashConfiguration().ByteArrayToHexStringWithPrefix();
            return Task.FromResult(imageHash);
        }

        private List<Leaf> ParseContentToLeafs(string[] elements)
        {
            var leaves = new List<Leaf>();
            var index = 0;

            while (index < elements.Length)
            {
                string firstElement = elements[index];
                string firstElementType = firstElement.Split(':')[0];

                if (firstElementType == "signer")
                {
                    var parts = firstElement.Split(':');
                    string address = parts[1];
                    BigInteger weight = BigInteger.Parse(parts[2]);

                    leaves.Add(new SignerLeaf
                    {
                        address = new Address(address),
                        weight = weight
                    });

                    index++;
                }
                else if (firstElementType == SubdigestLeaf.type)
                {
                    var parts = firstElement.Split(':');
                    string digest = parts[1];

                    leaves.Add(new SubdigestLeaf
                    {
                        digest = digest.HexStringToByteArray()
                    });

                    index++;
                }
                else if (firstElementType == AnyAddressSubdigestLeaf.type)
                {
                    var parts = firstElement.Split(':');
                    string digest = parts[1];

                    leaves.Add(new AnyAddressSubdigestLeaf
                    {
                        digest = digest.HexStringToByteArray()
                    });

                    index++;
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

                    index++;
                }
                else if (firstElementType == NestedLeaf.type)
                {
                    var parts = firstElement.Split(':');
                    BigInteger threshold = BigInteger.Parse(parts[1]);
                    BigInteger weight = BigInteger.Parse(parts[2]);

                    if (index + 1 >= elements.Length || !elements[index + 1].StartsWith("("))
                        throw new Exception("Missing nested element group after 'nested'");

                    string nestedGroup = elements[index + 1];
                    if (!nestedGroup.EndsWith(")"))
                        throw new Exception("Unclosed nested group: " + nestedGroup);

                    string inner = nestedGroup.Substring(1, nestedGroup.Length - 2);
                    string[] innerElements = inner.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    leaves.Add(new NestedLeaf
                    {
                        threshold = threshold,
                        weight = weight,
                        tree = Topology.FromLeaves(ParseContentToLeafs(innerElements))
                    });

                    index++;
                }
                else if (firstElementType == "node")
                {
                    var parts = firstElement.Split(':');
                    string hash = parts[1];

                    leaves.Add(new NodeLeaf
                    {
                        Value = hash.HexStringToByteArray()
                    });

                    index++;
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