namespace Sequence.WaaS
{
    [System.Serializable]
    public class RemovePartnerWalletSignerArgs
    {
        public PartnerWalletSigner signer;

        public RemovePartnerWalletSignerArgs(PartnerWalletSigner signer)
        {
            this.signer = signer;
        }
    }
}