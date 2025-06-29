using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class DevToolsTest
    {
        private class RandomOptions
        {
            public Random random = new ();
            public Func<double> seededRandom;
            public string checkpointer;
            public BigInteger minThresholdOnNested;
            public BigInteger maxPermissions;
            public string skewed;
        }
        
        public Task<string> DevToolsRandomConfig(Dictionary<string, object> parameters)
        {
            var maxDepth = int.Parse(parameters["maxDepth"].ToString());
            var seed = (string)parameters["seed"];
            var minThresholdOnNested = int.Parse(parameters["minThresholdOnNested"].ToString());
            var checkpointer = parameters.TryGetValue("checkpointer", out var checkpointerObj) ? checkpointerObj.ToString() : "no";
            var skewed = parameters.TryGetValue("skewed", out var skewedObj) ? skewedObj.ToString() : "none";

            var options = new RandomOptions
            {
                seededRandom = string.IsNullOrEmpty(seed) ? null : CreateSeededRandom(seed),
                checkpointer = checkpointer,
                minThresholdOnNested = minThresholdOnNested,
                skewed = skewed
            };
            
            return Task.FromResult(CreateRandomConfig(maxDepth, options));
        }
        
        public Task<string> DevToolsRandomSessionTopology(Dictionary<string, object> parameters)
        {
            throw new System.NotImplementedException("DevToolsTest.DevToolsRandomSessionTopology");
        }

        private string CreateRandomConfig(int maxDepth, RandomOptions options)
        {
            return new Primitives.Config
            {
                threshold = RandomBigInt(100, options),
                checkpoint = RandomBigInt(1000, options),
                topology = GenerateRandomTopology(maxDepth, options),
                checkpointer = options?.checkpointer switch
                {
                    "yes" => RandomAddress(options),
                    "random" => (options?.seededRandom?.Invoke() ?? options.random.NextDouble()) > 0.5
                        ? RandomAddress(options)
                        : null,
                    "no" or null => null,
                    _ => null
                }
            }.ToJson();
        }
        
        private static BigInteger RandomBigInt(int max, RandomOptions options)
        {
            var rnd = options.seededRandom?.Invoke() ?? options.random.NextDouble();
            return new BigInteger((long)(rnd * max));
        }

        private static Address RandomAddress(RandomOptions options)
        {
            return new Address(RandomHex(20, options));
        }
        
        private static string RandomHex(int byteLength, RandomOptions options)
        {
            var bytes = new byte[byteLength];
            options.random.NextBytes(bytes);
            return "0x" + BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        
        public static Func<double> CreateSeededRandom(string seed)
        {
            string currentSeed = seed;
            byte[] hash = ComputeSha256(currentSeed);
            int index = 0;

            return () =>
            {
                if (index >= hash.Length - 4)
                {
                    currentSeed += "1";
                    hash = ComputeSha256(currentSeed);
                    index = 0;
                }

                uint value = BitConverter.ToUInt32(hash, index);
                index += 4;

                return value / (double)uint.MaxValue;
            };
        }

        private static byte[] ComputeSha256(string input)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        }
        
        private static Topology GenerateRandomTopology(int depth, RandomOptions options)
        {
            if (depth <= 0)
            {
                var leafType = (int)Math.Floor((options.seededRandom?.Invoke() ?? options.random.NextDouble()) * 5);
                switch (leafType)
                {
                    case 0:
                        return new Topology(new SignerLeaf
                        {
                            address = RandomAddress(options),
                            weight = RandomBigInt(256, options)
                        });

                    case 1:
                        return new Topology(new SapientSignerLeaf
                        {
                            address = RandomAddress(options),
                            weight = RandomBigInt(256, options),
                            imageHash = RandomHex(32, options)
                        });

                    case 2:
                        return new Topology(new SubdigestLeaf
                        {
                            digest = RandomHex(32, options).HexStringToByteArray()
                        });

                    case 3:
                        return new Topology(new NodeLeaf
                        {
                            Value = RandomHex(32, options).HexStringToByteArray()
                        });

                    case 4:
                        BigInteger minThreshold = (BigInteger)options?.minThresholdOnNested;
                        return new Topology(new NestedLeaf
                        {
                            tree = GenerateRandomTopology(0, options),
                            weight = RandomBigInt(256, options),
                            threshold = minThreshold + RandomBigInt(65535 - (int)minThreshold, options)
                        });
                }
            }

            if (options?.skewed == "left")
            {
                return new Topology(new Node(
                    GenerateRandomTopology(0, options),
                    GenerateRandomTopology(depth - 1, options)));
            }
            
            if (options?.skewed == "right")
            {
                return new Topology(new Node(
                    GenerateRandomTopology(depth - 1, options),
                    GenerateRandomTopology(0, options)));
            }
            
            return new Topology(new Node(
                GenerateRandomTopology(depth - 1, options),
                GenerateRandomTopology(depth - 1, options)));
        }
    }
}