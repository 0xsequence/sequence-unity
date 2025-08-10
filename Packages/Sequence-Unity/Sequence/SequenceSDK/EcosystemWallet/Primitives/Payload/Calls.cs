using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class Calls : Payload
    {
        public static readonly Dictionary<string, NamedType[]> Types = new Dictionary<string, NamedType[]>()
        {
            ["Calls"] = new[]
            {
                new NamedType("calls", "Call[]"),
                new NamedType("space", "uint256"),
                new NamedType("nonce", "uint256"),
                new NamedType("wallets", "address[]"),
            },
            ["Call"] = new[]
            {
                new NamedType("to", "address"),
                new NamedType("value", "uint256"),
                new NamedType("data", "bytes"),
                new NamedType("gasLimit", "uint256"),
                new NamedType("delegateCall", "bool"),
                new NamedType("onlyFallback", "bool"),
                new NamedType("behaviorOnError", "uint256"),
            }
        };
        
        public const uint MaxNonceBytes = 15;
        public const uint MaxCalls = 65536;
        
        public override PayloadType type => PayloadType.Call;

        public BigInteger space;
        public BigInteger nonce;
        public Call[] calls;

        [Preserve]
        public Calls(BigInteger space, BigInteger nonce, Call[] calls)
        {
            this.space = space;
            this.nonce = nonce;
            this.calls = calls;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Calls))
            {
                return false;
            }
            
            Calls other = (Calls)obj;
            if (space != other.space || nonce != other.nonce || calls.Length != other.calls.Length)
            {
                return false;
            }
            for (int i = 0; i < calls.Length; i++)
            {
                if (!calls[i].Equals(other.calls[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override byte[] GetEIP712EncodeData()
        {
            byte[] spaceEncoded = new NumberCoder().Encode(space);
            byte[] nonceEncoded = new NumberCoder().Encode(nonce);
            byte[] callsEncoded = new byte[] { };
            if (calls != null)
            {
                foreach (var call in calls)
                {
                    callsEncoded = ByteArrayExtensions.ConcatenateByteArrays(callsEncoded, call.HashStruct());
                }
            }
            callsEncoded = SequenceCoder.KeccakHash(callsEncoded);
            byte[] encoded = ByteArrayExtensions.ConcatenateByteArrays(
                spaceEncoded,
                nonceEncoded,
                callsEncoded
            );
            return encoded;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.AppendLine($"Calls {{ space: {space}, nonce: {nonce}, calls: [");
            
            if (calls != null)
            {
                for (int i = 0; i < calls.Length; i++)
                {
                    sb.AppendLine($"    {calls[i]},");
                }
            }
            
            string parentWalletsStr = parentWallets != null ? string.Join(", ", parentWallets.Select(w => w.ToString()).ToArray()) : "null";
            sb.AppendLine($"], parentWallets: [{parentWalletsStr}] }}");
            
            return sb.ToString();
        }

        // Todo: once we're able to test the following code, let's refactor it into separate classes and methods instead of giant methods
        
        public byte[] Encode(Address self = null)
        {
            CallsEncoder encoder = new CallsEncoder(self, this);
            return encoder.Encode();
        }

        public static Calls Decode(byte[] packed, Address self = null)
        {
            CallsDecoder decoder = new CallsDecoder(packed);
            return decoder.Decode(self);
        }
        
        internal static Calls FromSolidityEncoding(SolidityDecoded decoded)
        {
            return new Calls(decoded.space, decoded.nonce, decoded.calls);
        }
    }
}