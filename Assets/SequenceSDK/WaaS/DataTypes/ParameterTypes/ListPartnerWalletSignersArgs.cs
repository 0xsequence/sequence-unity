namespace Sequence.WaaS
{
    [System.Serializable]
    public class ListPartnerWalletSignersArgs
    {
        public uint partnerId;

        public ListPartnerWalletSignersArgs(uint partnerId)
        {
            this.partnerId = partnerId;
        }
    }
}