using System;
using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public class Weigth
    {
        public BigInteger weight;
        public BigInteger maxWeight;
        
        public static Weigth GetWeight(Topology topology, Func<Leaf, bool> canSign)
        {
            if (topology == null)
            {
                return new Weigth { weight = 0, maxWeight = 0 };
            }

            if (topology.IsNode())
            {
                var left = GetWeight(topology.Node.left, canSign);
                var right = GetWeight(topology.Node.right, canSign);
                return new Weigth 
                { 
                    weight = left.weight + right.weight, 
                    maxWeight = left.maxWeight + right.maxWeight 
                };
            }
            else if (topology.IsLeaf())
            {
                return GetWeightForLeaf(topology.Leaf, canSign);
            }

            return new Weigth { weight = 0, maxWeight = 0 };
        }

        public static Weigth GetWeight(Config configuration, Func<Leaf, bool> canSign)
        {
            if (configuration?.topology == null)
            {
                return new Weigth { weight = 0, maxWeight = 0 };
            }

            return GetWeight(configuration.topology, canSign);
        }

        // Todo once tests are passing refactor to get the weight on the leafs directly, we can create an abstract method and overwrite it
        private static Weigth GetWeightForLeaf(Leaf leaf, Func<Leaf, bool> canSign)
        {
            if (leaf is SignedSignerLeaf signedSigner)
            {
                return new Weigth { weight = signedSigner.weight, maxWeight = signedSigner.weight };
            }
            if (leaf is SignedSapientSignerLeaf signedSapient)
            {
                return new Weigth { weight = signedSapient.weight, maxWeight = signedSapient.weight };
            }
            if (leaf is SignerLeaf signer)
            {
                BigInteger maxWeight = canSign(signer) ? signer.weight : 0;
                return new Weigth { weight = 0, maxWeight = maxWeight };
            }
            if (leaf is SapientSignerLeaf sapient)
            {
                BigInteger maxWeight = canSign(sapient) ? sapient.weight : 0;
                return new Weigth { weight = 0, maxWeight = maxWeight };
            }
            if (leaf is SubdigestLeaf || leaf is AnyAddressSubdigestLeaf || leaf is NodeLeaf)
            {
                return new Weigth { weight = 0, maxWeight = 0 };
            }
            if (leaf is NestedLeaf rawNested)
            {
                var result = GetWeight(rawNested.tree, canSign);
                BigInteger weight = result.weight >= rawNested.threshold ? rawNested.weight : 0;
                BigInteger maxWeight = result.maxWeight >= rawNested.threshold ? rawNested.weight : 0;
                return new Weigth { weight = weight, maxWeight = maxWeight };
            }

            return new Weigth { weight = 0, maxWeight = 0 };
        }
    }
}