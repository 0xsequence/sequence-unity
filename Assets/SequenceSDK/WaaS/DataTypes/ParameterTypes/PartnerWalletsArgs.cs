namespace Sequence.WaaS
{
    [System.Serializable]
    public class PartnerWalletsArgs
    {
        public uint partnerId;
        public Page page;

        public PartnerWalletsArgs(uint partnerId, Page page = null)
        {
            this.partnerId = partnerId;
            this.page = page;
        }
    }
}