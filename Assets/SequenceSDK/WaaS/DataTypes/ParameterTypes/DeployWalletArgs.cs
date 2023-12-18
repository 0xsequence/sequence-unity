namespace Sequence.WaaS
{
    [System.Serializable]
    public class DeployWalletArgs
    {
        public uint chainId;
        public uint accountIndex;

        public DeployWalletArgs(uint chainId, uint accountIndex)
        {
            this.chainId = chainId;
            this.accountIndex = accountIndex;
        }
    }
}