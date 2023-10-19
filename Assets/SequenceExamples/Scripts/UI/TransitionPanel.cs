using System;
using Sequence.Utils;

namespace Sequence.Demo
{
    public class TransitionPanel : UIPanel
    {
        public ITokenContentFetcher TokenFetcher = new TokenContentFetcher(contentFetcher);
        public INftContentFetcher NftFetcher = new NftContentFetcher(contentFetcher);
        private SequenceSampleUI _ui;
        private static IContentFetcher contentFetcher =
            new ContentFetcher(new Address("0x8e3e38fe7367dd3b52d1e281e4e8400447c8d8b9"),
                EnumExtensions.GetEnumValuesAsList<Chain>().ToArray());
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