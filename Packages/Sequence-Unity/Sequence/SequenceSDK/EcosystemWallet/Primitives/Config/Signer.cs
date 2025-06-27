using System;
using System.Collections.Generic;
using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public class Signer
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
                    if (top.Leaf is SignerLeaf signerLeaf)
                    {
                        if (signerLeaf.weight > 0)
                        {
                            signers.Add(signerLeaf.address);
                        }
                    }
                    else if (top.Leaf is SapientSignerLeaf sapientSignerLeaf)
                    {
                        sapientSigners.Add(new SapientSigner 
                        { 
                            address = sapientSignerLeaf.address, 
                            imageHash = sapientSignerLeaf.imageHash 
                        });
                    }
                    else if (top.Leaf is NodeLeaf)
                    {
                        isComplete = false;
                    }
                    else if (top.Leaf is NestedLeaf nestedLeaf)
                    {
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