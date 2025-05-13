using System;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    internal class Parent
    {
        public Address[] parentWallets;

        [Preserve]
        public Parent(Address[] parentWallets)
        {
            this.parentWallets = parentWallets;
        }
    }

    [Serializable]
    internal class Parented
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
            foreach (var parentWallet in parentWallets)
            {
                parentWalletsEncoded =
                    ByteArrayExtensions.ConcatenateByteArrays(new AddressCoder().Encode(parentWalletsEncoded));
            }
            parentWalletsEncoded = SequenceCoder.KeccakHash(parentWalletsEncoded);
            
            byte[] encoded = ByteArrayExtensions.ConcatenateByteArrays(payloadEncoded, parentWalletsEncoded);
            return encoded;
        }
    }
}