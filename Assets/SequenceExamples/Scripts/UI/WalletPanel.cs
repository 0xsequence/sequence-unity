using System.Collections;
using Sequence.Utils;

namespace Sequence.Demo
{
    public class WalletPanel : UIPanel
    {
        private WalletPage _walletPage;
        private TransitionPanel _transitionPanel;

        protected override void Awake()
        {
            base.Awake();
            _walletPage = GetComponentInChildren<WalletPage>();
            _transitionPanel = FindObjectOfType<TransitionPanel>();
        }

        public override void Close()
        {
            base.Close();
            _walletPage.Close();
        }

        public override IEnumerator OpenInitialPage(params object[] openArgs)
        {
            openArgs = SetupContentFetchers(openArgs);
            return base.OpenInitialPage(openArgs);
        }

        private object[] SetupContentFetchers(params object[] args)
        {
            ITokenContentFetcher tokenFetcher = args.GetObjectOfTypeIfExists<ITokenContentFetcher>();
            if (tokenFetcher == null)
            {
                args.AppendObject(new MockTokenContentFetcher());
            }

            INftContentFetcher nftFetcher = args.GetObjectOfTypeIfExists<INftContentFetcher>();
            if (nftFetcher == null)
            {
                args.AppendObject(new MockNftContentFetcher());
            }

            return args;
        }

        public void OpenTransitionPanel()
        {
            _transitionPanel.OpenWithDelay(_closeAnimationDurationInSeconds);
        }
    }
}