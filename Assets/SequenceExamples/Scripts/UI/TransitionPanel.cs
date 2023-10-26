namespace Sequence.Demo
{
    public class TransitionPanel : UIPanel
    {
        public ITokenContentFetcher TokenFetcher = new MockTokenContentFetcher(); // Todo inject a real fetcher
        public INftContentFetcher NftFetcher = new MockNftContentFetcher(30); // Todo inject a real fetcher
        private SequenceSampleUI _ui;
        protected override void Awake()
        {
            base.Awake();
            _ui = FindObjectOfType<SequenceSampleUI>();
        }

        public void OpenWalletPanel()
        {
            _ui.OpenWalletPanelWithDelay(_closeAnimationDurationInSeconds, TokenFetcher, NftFetcher, 
                new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"));
        }
    }
}