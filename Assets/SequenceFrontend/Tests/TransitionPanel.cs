using System;
using Sequence.Authentication;
using Sequence.Contracts;
using Sequence.Demo;
using Sequence.Utils;
using Sequence.EmbeddedWallet;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class TransitionPanel : UIPanel
    {
        public ITokenContentFetcher TokenFetcher = new TokenContentFetcher(contentFetcher);
        public INftContentFetcher NftFetcher = new NftContentFetcher(contentFetcher);
        public IWallet Wallet;
        private SequenceSampleUI _ui;
        private static IContentFetcher contentFetcher =
            new ContentFetcher(new Address("0x8e3e38fe7367dd3b52d1e281e4e8400447c8d8b9"),
                EnumExtensions.GetEnumValuesAsList<Chain>().ToArray());

        private string _easyMintFungibleTokenContract = "0x9d0d8dcba30c8b7241da84f922942c100eb1bddc";
        
        protected override void Awake()
        {
            base.Awake();
            _ui = FindObjectOfType<SequenceSampleUI>();
            SequenceWallet.OnWalletCreated += (wallet =>
            {
                Wallet = wallet;
            });
        }

        public void OpenWalletPanel()
        {
            _ui.OpenWalletPanelWithDelay(_closeAnimationDurationInSeconds, TokenFetcher, NftFetcher, 
                Wallet);
        }
        
        public void OpenPlayerProfilePanel()
        {
            gameObject.SetActive(false);
            _ui.OpenPlayerProfile(Wallet);
        }
        
        public void OpenDailyRewardsPanel()
        {
            gameObject.SetActive(false);
            _ui.OpenDailyRewards(Wallet);
        }
        
        public void OpenInventoryPanel()
        {
            gameObject.SetActive(false);
            _ui.OpenInventory(Wallet);
        }
        
        public void OpenInGameShopPanel()
        {
            gameObject.SetActive(false);
            _ui.OpenInGameShop(Wallet);
        }
        
        public async void LinkEOAWallet()
        {
            var linker = new EOAWalletLinker(Wallet, Chain.ArbitrumNova);
            await linker.OpenEoaWalletLink();
        }

        public void SignOut()
        {
            Wallet.DropThisSession();
        }
    }
}