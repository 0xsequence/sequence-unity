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
            object[] decoded = ABI.ABI.Decode<object[]>(solidityEncodedPayload, DecodedAbi);
            return new SolidityDecoded(
                kind : (Kind)(int)decoded[0],
                noChainId : (bool)decoded[1],
                calls : ((object[])decoded[2]).ConvertToTArray<Call, object[]>(),
                space : (BigInteger)decoded[3],
                nonce : (BigInteger)decoded[4],
                message : (string)decoded[5],
                imageHash : (string)decoded[6],
                digest : (string)decoded[7],
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
    }
}