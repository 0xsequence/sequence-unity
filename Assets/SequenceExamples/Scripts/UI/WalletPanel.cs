using System.Collections;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Demo
{
    public class WalletPanel : UIPanel
    {
        [SerializeField] private GameObject _searchButton;
        [SerializeField] private GameObject _backButton;
        
        private WalletPage _walletPage;
        private TransitionPanel _transitionPanel;

        public enum TopBarMode
        {
            Search,
            Back
        }

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

        public void SetTopBarMode(TopBarMode mode)
        {
            switch (mode)
            {
                case TopBarMode.Search:
                    _searchButton.SetActive(true);
                    _backButton.SetActive(false);
                    break;
                case TopBarMode.Back:
                    _searchButton.SetActive(false);
                    _backButton.SetActive(true);
                    break;
            }
        }
    }
}