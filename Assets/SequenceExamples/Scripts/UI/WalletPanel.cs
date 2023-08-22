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
            _walletPage.SetupContentFetchers(new MockContentFetcher());
            return base.OpenInitialPage(openArgs);
        }
    }
}