namespace Sequence.Demo
{
    public class TransitionPanel : UIPanel
    {
        public ITokenContentFetcher TokenFetcher = new MockTokenContentFetcher(); // Todo inject a real fetcher in Awake
        public INftContentFetcher NftFetcher = new MockNftContentFetcher(30); // Todo inject a real fetcher in Awake
        private SequenceUI _ui;
        protected override void Awake()
        {
            base.Awake();
            _ui = FindObjectOfType<SequenceUI>();
        }

        public void OpenWalletPanel()
        {
            _ui.OpenWalletPanelWithDelay(_closeAnimationDurationInSeconds, TokenFetcher, NftFetcher);
        }
    }
}