namespace Sequence.WaaS
{
    [System.Serializable]
    public class DeployPartnerParentWalletArgs
    {
        public uint partnerId;
        public uint chainId;

        public DeployPartnerParentWalletArgs(uint partnerId, uint chainId)
        {
            this.partnerId = partnerId;
            this.chainId = chainId;
        }
    }
}