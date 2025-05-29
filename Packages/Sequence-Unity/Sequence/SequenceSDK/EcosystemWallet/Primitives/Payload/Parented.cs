using System;
using System.Numerics;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class Parented
    {
        public Payload payload;
        public Address[] parentWallets;

        [Preserve]
        public Parented(Address[] parentWallets, Payload payload)
        {
            this.parentWallets = parentWallets;
            this.payload = payload;
        }

        public byte[] GetEIP712EncodeData()
        {
            byte[] payloadEncoded = payload.GetEIP712EncodeData();
            byte[] parentWalletsEncoded = new byte[] { };
            if (parentWallets != null)
            {
                foreach (var parentWallet in parentWallets)
                {
                    parentWalletsEncoded =
                        ByteArrayExtensions.ConcatenateByteArrays(parentWalletsEncoded, new AddressCoder().Encode(parentWallet));
                }
            }

            parentWalletsEncoded = SequenceCoder.KeccakHash(parentWalletsEncoded);
            
            byte[] encoded = ByteArrayExtensions.ConcatenateByteArrays(payloadEncoded, parentWalletsEncoded);
            return encoded;
        }
        
        public byte[] Hash(Address wallet, Chain chain)
        {
            TypedDataToSign typedData = new TypedDataToSign(wallet, chain, this);
            return typedData.GetSignPayload();
        }
        
        public EncodeSapient DoEncodeSapient(Chain chain)
        {
            EncodeSapient encoded = new EncodeSapient
            {
                kind = (int)payload.type,
                noChainId = chain != Chain.None,
                calls = new EncodeSapient.EncodedCall[]{},
                space = BigInteger.Zero,
                nonce = BigInteger.Zero,
                message = "0x",
                imageHash = "0x0000000000000000000000000000000000000000000000000000000000000000",
                digest = "0x0000000000000000000000000000000000000000000000000000000000000000",
                parentWallets = payload.parentWallets,
            };
            
            switch (payload)
            {
                case Calls callsPayload:
                    int callsLength = callsPayload.calls.Length;
                    encoded.calls = new EncodeSapient.EncodedCall[callsLength];
                    for (int i = 0; i < callsLength; i++)
                    {
                        Call call = callsPayload.calls[i];
                        encoded.calls[i] = new EncodeSapient.EncodedCall(call);
                    }
                    encoded.space = callsPayload.space;
                    encoded.nonce = callsPayload.nonce;
                    break;

                case Message messagePayload:
                    encoded.message = messagePayload.message.ByteArrayToHexStringWithPrefix();
                    break;

                case ConfigUpdate configPayload:
                    encoded.imageHash = configPayload.imageHash;
                    break;

                case Digest digestPayload:
                    encoded.digest = digestPayload.digest;
                    break;
            }

            return encoded;
        }

        public static Parented DecodeFromSolidityEncoding(string solidityEncodedPayload)
        {
            SolidityDecoded decoded = SolidityDecoded.FromSolidityEncoding(solidityEncodedPayload);
            return DecodeFromSolidityDecoded(decoded);
        }

        public static Parented DecodeFromSolidityDecoded(SolidityDecoded decoded)
        {
            Address[] parentWallets = decoded.parentWallets;
            Payload payload = null;
            switch (decoded.kind)
            {
                case SolidityDecoded.Kind.Transaction:
                    payload = Calls.FromSolidityEncoding(decoded);
                    break;
                case SolidityDecoded.Kind.Message:
                    payload = Message.FromSolidityEncoding(decoded);
                    break;
                case SolidityDecoded.Kind.ConfigUpdate:
                    payload = ConfigUpdate.FromSolidityEncoding(decoded);
                    break;
                case SolidityDecoded.Kind.Digest:
                    payload = Digest.FromSolidityEncoding(decoded);
                    break;
                default:
                    throw new NotImplementedException($"Unknown payload type: {decoded.kind}");
            }
            return new Parented(parentWallets, payload);
        }
    }
}