using System;
using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public class WeightCalculator
    {
        public struct WeightResult
        {
            public BigInteger weight;
            public BigInteger maxWeight;
        }

        public static WeightResult GetWeight(Topology topology, Func<Leaf, bool> canSign)
        {
            if (topology == null)
            {
                return new WeightResult { weight = 0, maxWeight = 0 };
            }

            if (topology.IsNode())
            {
                var left = GetWeight(topology.Node.left, canSign);
                var right = GetWeight(topology.Node.right, canSign);
                return new WeightResult 
                { 
                    weight = left.weight + right.weight, 
                    maxWeight = left.maxWeight + right.maxWeight 
                };
            }
            else if (topology.IsLeaf())
            {
                return GetWeightForLeaf(topology.Leaf, canSign);
            }

            return new WeightResult { weight = 0, maxWeight = 0 };
        }

        public static WeightResult GetWeight(Config configuration, Func<Leaf, bool> canSign)
        {
            if (configuration?.topology == null)
            {
                return new WeightResult { weight = 0, maxWeight = 0 };
            }

            return GetWeight(configuration.topology, canSign);
        }

        private static WeightResult GetWeightForLeaf(Leaf leaf, Func<Leaf, bool> canSign)
        {
            // Handle signed leaves (they have weight)
            if (leaf is SignedSignerLeaf signedSigner)
            {
                return new WeightResult { weight = signedSigner.weight, maxWeight = signedSigner.weight };
            }
            else if (leaf is SignedSapientSignerLeaf signedSapient)
            {
                return new WeightResult { weight = signedSapient.weight, maxWeight = signedSapient.weight };
            }
            else if (leaf is RawSignerLeaf rawSigner)
            {
                return new WeightResult { weight = rawSigner.weight, maxWeight = rawSigner.weight };
            }
            // Handle unsigned leaves that can potentially be signed
            else if (leaf is SignerLeaf signer)
            {
                BigInteger maxWeight = canSign(signer) ? signer.weight : 0;
                return new WeightResult { weight = 0, maxWeight = maxWeight };
            }
            else if (leaf is SapientSignerLeaf sapient)
            {
                BigInteger maxWeight = canSign(sapient) ? sapient.weight : 0;
                return new WeightResult { weight = 0, maxWeight = maxWeight };
            }
            // Handle other leaf types
            else if (leaf is SubdigestLeaf || leaf is AnyAddressSubdigestLeaf || leaf is NodeLeaf)
            {
                return new WeightResult { weight = 0, maxWeight = 0 };
            }
            else if (leaf is NestedLeaf rawNested)
            {
                var result = GetWeight(rawNested.tree, canSign);
                BigInteger weight = result.weight >= rawNested.threshold ? rawNested.weight : 0;
                BigInteger maxWeight = result.maxWeight >= rawNested.threshold ? rawNested.weight : 0;
                return new WeightResult { weight = weight, maxWeight = maxWeight };
            }

            return new WeightResult { weight = 0, maxWeight = 0 };
        }
    }
} 