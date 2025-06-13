using System;
using System.Collections.Generic;
using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class Signer
    {
        public Address[] signers;
        public SapientSigner[] sapientSigners;
        public bool isComplete;

        public static Signer[] GetSigners(Config configuration)
        {
            if (configuration == null || configuration.topology == null)
            {
                return null;
            }

            return GetSigners(configuration.topology);
        }

        public static Signer[] GetSigners(Topology topology)
        {
            if (topology == null)
            {
                return null;
            }

            var signers = new List<Address>();
            var sapientSigners = new List<SapientSigner>();
            bool isComplete = true;

            void Scan(Topology top)
            {
                if (top.IsNode())
                {
                    Scan(top.Node.left);
                    Scan(top.Node.right);
                }
                else if (top.IsLeaf())
                {
                    if (top.Leaf.isSignerLeaf)
                    {
                        SignerLeaf signerLeaf = (SignerLeaf)top.Leaf;
                        if (signerLeaf.weight > 0)
                        {
                            signers.Add(signerLeaf.address);
                        }
                    }
                    else if (top.Leaf.isSapientSignerLeaf)
                    {
                        SapientSignerLeaf sapientLeaf = (SapientSignerLeaf)top.Leaf;
                        sapientSigners.Add(new SapientSigner 
                        { 
                            address = sapientLeaf.address, 
                            imageHash = sapientLeaf.imageHash 
                        });
                    }
                    else if (top.Leaf.isNodeLeaf)
                    {
                        isComplete = false;
                    }
                    else if (top.Leaf.isNestedLeaf)
                    {
                        NestedLeaf nestedLeaf = (NestedLeaf)top.Leaf;
                        if (nestedLeaf.weight > 0)
                        {
                            Scan(nestedLeaf.tree);
                        }
                    }
                }
            }

            Scan(topology);
            
            return new Signer[]
            {
                new Signer
                {
                    signers = signers.ToArray(),
                    sapientSigners = sapientSigners.ToArray(),
                    isComplete = isComplete
                }
            };
        }
    }
}