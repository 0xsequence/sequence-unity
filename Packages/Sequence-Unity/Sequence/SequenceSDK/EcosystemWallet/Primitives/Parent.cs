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
}