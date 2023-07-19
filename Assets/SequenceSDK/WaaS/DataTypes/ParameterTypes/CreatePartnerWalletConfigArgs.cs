

namespace Sequence.WaaS
{
    [System.Serializable]
    public class CreatePartnerWalletConfigArgs
    {
        public uint partnerId;
        public string config;

        public CreatePartnerWalletConfigArgs(uint partnerId, string config)
        {
            this.partnerId = partnerId;
            this.config = config;
        }
    }
}