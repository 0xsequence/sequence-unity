namespace Sequence.WaaS
{
    [System.Serializable]
    public class PartnerWalletConfigArgs
    {
        public uint partnerId;

        public PartnerWalletConfigArgs(uint partnerId)
        {
            this.partnerId = partnerId;
        }
    }
}