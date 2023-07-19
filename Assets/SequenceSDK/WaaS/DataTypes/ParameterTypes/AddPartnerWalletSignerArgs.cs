namespace Sequence.WaaS
{
    [System.Serializable]
    public class AddPartnerWalletSignerArgs
    {
        public PartnerWalletSigner signer;

        public AddPartnerWalletSignerArgs(PartnerWalletSigner signer)
        {
            this.signer = signer;
        }
    }
}