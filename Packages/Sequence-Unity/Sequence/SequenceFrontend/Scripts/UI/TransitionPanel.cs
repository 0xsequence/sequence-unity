using System;
using Sequence.Utils;
using Sequence.WaaS;

namespace Sequence.Demo
{
    public class TransitionPanel : UIPanel
    {
        public ITokenContentFetcher TokenFetcher = new TokenContentFetcher(contentFetcher);
        public INftContentFetcher NftFetcher = new NftContentFetcher(contentFetcher);
        public IWallet Wallet;
        private SequenceSampleUI _ui;
        private static IContentFetcher contentFetcher =
            new ContentFetcher(new Address("0x8e3e38fe7367dd3b52d1e281e4e8400447c8d8b9"),
                EnumExtensions.GetEnumValuesAsList<Chain>().ToArray());
        
        protected override void Awake()
        {
            base.Awake();
            _ui = FindObjectOfType<SequenceSampleUI>();
            WaaS.Wallet.OnWalletCreated += (wallet =>
            {
                Wallet = wallet;
            });
        }

        public void OpenWalletPanel()
        {
            _ui.OpenWalletPanelWithDelay(_closeAnimationDurationInSeconds, TokenFetcher, NftFetcher, 
                Wallet);
        }

        public void OpenSignMessagePanel()
        {
            _ui.OpenSignMessagePanelWithDelay(_closeAnimationDurationInSeconds, Wallet, this);
        }
        
        public void OpenSendTransactionPanel()
        {
            _ui.OpenSendTransactionPanelWithDelay(_closeAnimationDurationInSeconds, Wallet, this);
        }

        public void OpenSendTransactionWithFeeOptionsPanel()
        {
            _ui.OpenSendTransactionWithFeeOptionsPanelWithDelay(_closeAnimationDurationInSeconds, Wallet, this);
        }
        
        public void LinkEOAWallet()
        {
            EoaWalletLinker linker = new EoaWalletLinker(Wallet, "https://demo-waas-wallet-link-server.tpin.workers.dev/generateNonce");
            linker.OpenEoaWalletLink(Chain.ArbitrumNova);
        }
    }
}