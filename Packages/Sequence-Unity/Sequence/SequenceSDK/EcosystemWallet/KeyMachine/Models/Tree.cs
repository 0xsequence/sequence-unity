namespace Sequence.EcosystemWallet.KeyMachine.Models
{
    internal struct TreeArgs
    {
        public string imageHash;

        public TreeArgs(string imageHash)
        {
            this.imageHash = imageHash;
        }
    }

    internal struct TreeReturn
    {
        public int version;
        public object tree;
    }
}