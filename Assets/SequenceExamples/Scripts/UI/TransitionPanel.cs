using System.Collections;
using UnityEngine;

namespace Sequence.Demo
{
    public class TransitionPanel : UIPanel
    {
        public ITokenContentFetcher TokenFetcher = new MockTokenContentFetcher(); // Todo inject a real fetcher in Awake
        public INftContentFetcher NftFetcher = new MockNftContentFetcher(30); // Todo inject a real fetcher in Awake
        private SequenceUI _ui;
        private WalletPanel _walletPanel;
        protected override void Awake()
        {
            base.Awake();
            _ui = FindObjectOfType<SequenceUI>();
            _walletPanel = FindObjectOfType<WalletPanel>();
        }

        public void OpenWalletPanel()
        {
            _walletPanel.OpenWithDelay(_closeAnimationDurationInSeconds, TokenFetcher, NftFetcher);
        }
    }
}