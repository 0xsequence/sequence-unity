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
        public Address factory;
        public string guestModule;
        public Address mainModule;
        public Address mainModuleUpgradable;
        public string walletCreationCode;
    }
}