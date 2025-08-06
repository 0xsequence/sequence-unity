namespace Sequence.EcosystemWallet.KeyMachine.Models
{
    internal struct ConfigArgs
    {
        public string imageHash;

        public ConfigArgs(string imageHash)
        {
            this.imageHash = imageHash;
        }
    }

    internal struct ConfigReturn
    {
        public int version;
        public object config;
    }
}