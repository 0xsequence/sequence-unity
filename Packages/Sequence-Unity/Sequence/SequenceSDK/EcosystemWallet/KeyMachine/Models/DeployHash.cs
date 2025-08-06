namespace Sequence.EcosystemWallet.KeyMachine.Models
{
    public struct DeployHashArgs
    {
        public Address wallet;

        public DeployHashArgs(Address wallet)
        {
            this.wallet = wallet;
        }
    }

    public struct DeployHashReturn
    {
        public string deployHash;
    }
}