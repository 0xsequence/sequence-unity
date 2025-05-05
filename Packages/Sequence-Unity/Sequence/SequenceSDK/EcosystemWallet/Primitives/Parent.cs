using System;
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
        public Address[] parentWallets;
        public Payload payload;

        [Preserve]
        public Parented(Address[] parentWallets, Payload payload)
        {
            this.parentWallets = parentWallets;
            this.payload = payload;
        }
    }
}