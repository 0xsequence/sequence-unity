using System;
using System.Numerics;
using System.Text;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignerLeaf : Leaf
    {
        public const string type = "signer";
        
        public Address address;
        public BigInteger weight;
        public bool signed;
        public SignatureType signature;
        
        public override object Parse()
        {
            return new
            {
                type = type,
                address = address,
                weight = weight.ToString()
            };
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
            byte[] prefix = Encoding.UTF8.GetBytes("Sequence signer:\n");
            byte[] address = this.address.Value.HexStringToByteArray();
            byte[] weight = this.weight.ByteArrayFromNumber().PadLeft(32);
                
            return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, address, weight));
        }
    }
}