namespace Sequence.EcosystemWallet.KeyMachine.Models
{
    internal struct DeployHashArgs
    {
        public Address wallet;

        public DeployHashArgs(Address wallet)
        {
            this.wallet = wallet;
        }
    }

    internal struct DeployHashReturn
    {
        public string deployHash;
        public DeployHashContext context;
    }

    internal struct DeployHashContext
    {
        public int version;
        public string factory;
        public string guestModule;
        public string mainModule;
        public string mainModuleUpgradable;
        public string walletCreationCode;
    }
}