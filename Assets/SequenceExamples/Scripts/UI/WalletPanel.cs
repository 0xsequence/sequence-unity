using System.Collections;

namespace Sequence.Demo
{
    public class WalletPanel : UIPanel
    {
        private WalletPage _walletPage;

        protected override void Awake()
        {
            base.Awake();
            _walletPage = GetComponentInChildren<WalletPage>();
        }

        public override IEnumerator OpenInitialPage(params object[] openArgs)
        {
            SetupContentFetchers(openArgs);
            return base.OpenInitialPage(openArgs);
        }

        private void SetupContentFetchers(params object[] args)
        {
            INftContentFetcher nftFetcher = new MockContentFetcher();
            if (args.Length > 0)
            {
                if (args[0] is INftContentFetcher fetcher)
                {
                    nftFetcher = fetcher;
                }
            }
            _walletPage.SetupContentFetchers(nftFetcher);
        }
    }
}