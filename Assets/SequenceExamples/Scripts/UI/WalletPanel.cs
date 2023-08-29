using System.Collections;
using Sequence.Utils;

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
            ITokenContentFetcher tokenFetcher = args.GetObjectOfTypeIfExists<ITokenContentFetcher>();
            if (tokenFetcher == null)
            {
                tokenFetcher = new MockTokenContentFetcher();
            }

            INftContentFetcher nftFetcher = args.GetObjectOfTypeIfExists<INftContentFetcher>();
            if (nftFetcher == null)
            {
                nftFetcher = new MockNftContentFetcher();
            }
            
            _walletPage.SetupContentFetchers(tokenFetcher, nftFetcher);
        }
    }
}