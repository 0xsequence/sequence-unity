using System;
using System.Numerics;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class SolidityDecoded
    {
        public Kind kind;
        public bool noChainId;
        public Call[] calls;
        public BigInteger space;
        public BigInteger nonce;
        public string message;
        public string imageHash;
        public string digest;
        public Address[] parentWallets;
        
        private const string DecodedAbi =
            "(uint8,bool,(address,uint256,bytes,uint256,bool,bool,uint256)[],uint256,uint256,bytes,bytes32,bytes32,address[])";

        [Preserve]
        public SolidityDecoded(Kind kind, bool noChainId, Call[] calls, BigInteger space, BigInteger nonce, string message, string imageHash, string digest, Address[] parentWallets)
        {
            this.kind = kind;
            this.noChainId = noChainId;
            this.calls = calls;
            this.space = space;
            this.nonce = nonce;
            this.message = message;
            this.imageHash = imageHash;
            this.digest = digest;
            this.parentWallets = parentWallets;
        }

        public static SolidityDecoded FromSolidityEncoding(string solidityEncodedPayload)
        {
            solidityEncodedPayload = solidityEncodedPayload.Replace("0x", "").Substring(64);
            object[] decoded = ABI.ABI.Decode<object[]>(solidityEncodedPayload, DecodedAbi);
            return new SolidityDecoded(
                kind : ToKind((BigInteger)decoded[0]),
                noChainId : (bool)decoded[1],
                calls : ToCalls((object[])decoded[2]),
                space : (BigInteger)decoded[3],
                nonce : (BigInteger)decoded[4],
                message : ((byte[])decoded[5]).ByteArrayToHexStringWithPrefix(),
                imageHash : ((byte[])decoded[6]).ByteArrayToHexString().PadRight(64, '0').EnsureHexPrefix(),
                digest : ((byte[])decoded[7]).ByteArrayToHexString().PadRight(64, '0').EnsureHexPrefix(),
                parentWallets : ((object[])decoded[8]).ConvertToTArray<Address, object[]>()
            );
        }

        public enum Kind
        {
            Transaction = 0x00,
            Message = 0x01,
            ConfigUpdate = 0x02,
            Digest = 0x03,
        }
        
        internal static Kind ToKind(BigInteger kind)
        {
            if (kind == BigInteger.Zero) return Kind.Transaction;
            if (kind == BigInteger.One) return Kind.Message;
            if (kind == new BigInteger(2)) return Kind.ConfigUpdate;
            if (kind == new BigInteger(3)) return Kind.Digest;

            throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }
        
        internal static Call[] ToCalls(object[] calls)
        {
            int length = calls.Length;
            Call[] callArray = new Call[length];
            for (int i = 0; i < length; i++)
            {
                object[] call = (object[])calls[i];
                callArray[i] = new Call(
                    to : (Address)call[0],
                    value : (BigInteger)call[1],
                    data : (byte[])call[2],
                    gasLimit : (BigInteger)call[3],
                    delegateCall : (bool)call[4],
                    onlyFallback : (bool)call[5],
                    behaviorOnError : ToBehaviourOnError((BigInteger)call[6])
                );
            }

            return callArray;
        }

        internal static BehaviourOnError ToBehaviourOnError(BigInteger behaviour)
        {
            if (behaviour == BigInteger.Zero) return BehaviourOnError.ignore;
            if (behaviour == BigInteger.One) return BehaviourOnError.revert;
            if (behaviour == new BigInteger(2)) return BehaviourOnError.abort;
            
            throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
        }
    }
}