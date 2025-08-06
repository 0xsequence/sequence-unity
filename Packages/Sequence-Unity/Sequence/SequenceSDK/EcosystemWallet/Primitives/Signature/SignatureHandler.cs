using System;
using Sequence.EcosystemWallet.Envelope;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignatureHandler
    {
        public static Topology FillLeaves(Topology topology, Func<Leaf, SignatureOfLeaf> signatureFor)
        {
            if (topology.IsNode())
            {
                return new Topology(new Node(
                    FillLeaves(topology.Node.left, signatureFor),
                    FillLeaves(topology.Node.right, signatureFor)));
            }
            
            if (!topology.IsLeaf())
                throw new ArgumentException("Cannot create topology from empty leaves");

            var leaf = topology.Leaf;
            if (leaf is SignerLeaf signerLeaf)
            {
                var signature = signatureFor(signerLeaf);
                var newLeaf = signature != null ? new SignedSignerLeaf
                {
                    address = signerLeaf.address,
                    weight = signerLeaf.weight,
                    signature = signature
                } : signerLeaf;
                
                return new Topology(newLeaf);
            }

            if (leaf is SapientSignerLeaf sapientSignerLeaf)
            {
                var signature = signatureFor(sapientSignerLeaf);
                var newLeaf = signature != null ? new SignedSapientSignerLeaf
                {
                    address = sapientSignerLeaf.address,
                    imageHash = sapientSignerLeaf.imageHash,
                    weight = sapientSignerLeaf.weight,
                    signature = signature
                } : sapientSignerLeaf;
                
                return new Topology(newLeaf);
            }

            if (leaf is NestedLeaf nestedLeaf)
            {
                return new Topology(new NestedLeaf
                {
                    weight = nestedLeaf.weight,
                    threshold = nestedLeaf.threshold,
                    tree = FillLeaves(nestedLeaf.tree, signatureFor)
                });
            }
            
            if (leaf is SubdigestLeaf ||
                leaf is AnyAddressSubdigestLeaf ||
                leaf is NodeLeaf)
            {
                return topology;
            }

            throw new Exception("Invalid topology");
        }

        public static RawSignature EncodeSignature(SignedEnvelope<Payload> envelope)
        {
            var topology = FillLeaves(envelope.configuration.topology, 
                leaf => SignatureForLeaf(envelope, leaf));

            return new RawSignature
            {
                noChainId = envelope.chainId == 0,
                configuration = new Config
                {
                    threshold = envelope.configuration.threshold,
                    checkpoint = envelope.configuration.checkpoint,
                    checkpointer = envelope.configuration.checkpointer,
                    topology = null,
                }
            };
        }

        public static EnvelopeSignature SignatureForLeaf(SignedEnvelope<Payload> envelope, Leaf leaf)
        {
            if (leaf is SignerLeaf signerLeaf)
            {
                return Array.Find(envelope.signatures, sig => 
                    sig is Signature signature &&
                    signature.address.Equals(signerLeaf.address));
            }   
            
            if (leaf is SapientSignerLeaf sapientSignerLeaf)
            {
                return Array.Find(envelope.signatures, sig =>
                    sig is SapientSignature sapient &&
                    sapient.imageHash == sapientSignerLeaf.imageHash &&
                    sapient.signature.address.Equals(sapientSignerLeaf.address));
            }

            return null;
        }
    }
}