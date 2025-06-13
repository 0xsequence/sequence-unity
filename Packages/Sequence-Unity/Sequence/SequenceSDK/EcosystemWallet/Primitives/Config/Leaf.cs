namespace Sequence.EcosystemWallet.Primitives
{
    internal abstract class Leaf : RawLeaf
    {
        public bool isSignerLeaf => this is SignerLeaf;
        public bool isSapientSignerLeaf => this is SapientSignerLeaf;
        public bool isSubdigestLeaf => this is SubdigestLeaf;
        public bool isAnyAddressSubdigestLeaf => this is AnyAddressSubdigestLeaf;
        public bool isNodeLeaf => this is NodeLeaf;
        public bool isNestedLeaf => this is NestedLeaf;
    }
}