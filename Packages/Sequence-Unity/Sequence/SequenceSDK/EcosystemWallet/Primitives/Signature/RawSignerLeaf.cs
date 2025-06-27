using System;
using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public class RawSignerLeaf : Leaf
    {
        public const string type = "unrecovered-signer";
        public BigInteger weight;
        public SignatureType signature;
        
        public override object Parse()
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            if (signature is SignatureOfSignerLeafHash hash)
            {
                return hash.Encode(this);
            }

            if (signature is SignatureOfSignerLeafEthSign ethSign)
            {
                return ethSign.Encode(this);
            }

            if (signature is SignatureOfSignerLeafErc1271 erc1271)
            {
                return erc1271.Encode(this);
            }

            if (signature is SignatureOfSapientSignerLeaf sapientSigner)
            {
                return sapientSigner.Encode(this);
            }
                
            throw new Exception($"Invalid signature type: {signature.type}");
        }

        public override byte[] HashConfiguration()
        {
            throw new System.NotImplementedException();
        }
    }

    public abstract class SignatureType
    {
        public abstract string type { get; }

        public abstract byte[] Encode(Leaf leaf);
    }
}