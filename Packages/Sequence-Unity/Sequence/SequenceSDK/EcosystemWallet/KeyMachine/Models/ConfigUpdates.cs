namespace Sequence.EcosystemWallet.KeyMachine.Models
{
    internal struct ConfigUpdatesArgs
    {
        public string wallet;
        public string fromImageHash;

        public ConfigUpdatesArgs(string wallet, string fromImageHash)
        {
            this.wallet = wallet;
            this.fromImageHash = fromImageHash;
        }
    }

    internal struct ConfigUpdatesReturn
    {
        public ConfigUpdate[] updates;
    }

    internal struct ConfigUpdate
    {
        public string toImageHash;
        public string signature;
    }
}