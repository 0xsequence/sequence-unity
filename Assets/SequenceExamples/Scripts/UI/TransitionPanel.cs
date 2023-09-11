namespace Sequence.Demo
{
    public class TransitionPanel : UIPanel
    {
        public ITokenContentFetcher TokenFetcher = new MockTokenContentFetcher(); // Todo inject a real fetcher in Awake
        public INftContentFetcher NftFetcher = new MockNftContentFetcher(30); // Todo inject a real fetcher in Awake
        private SequenceSampleUI _ui;
        protected override void Awake()
        {
            base.Awake();
            _ui = FindObjectOfType<SequenceSampleUI>();
        }

        public void OpenWalletPanel()
        {
            _ui.OpenWalletPanelWithDelay(_closeAnimationDurationInSeconds, TokenFetcher, NftFetcher);
        }
    }
}