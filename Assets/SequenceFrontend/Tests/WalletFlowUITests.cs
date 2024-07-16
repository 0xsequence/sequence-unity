using System;
using System.Collections;
using System.Collections.Generic;
using Sequence;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using SequenceExamples.Scripts.Tests.Utils;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SequenceExamples.Scripts.Tests
{
    public class WalletFlowUITests : MonoBehaviour
    {
        public int RandomNumberOfTokensToFetch;
        public int RandomNumberOfNftsToFetch;
        
        private MonoBehaviour _testMonobehaviour;
            
        private SequenceSampleUI _ui;
        private WalletPanel _walletPanel;
        private WalletPage _walletPage;
        private LoginPanel _loginPanel;
        private TransitionPanel _transitionPanel;
        private SearchPage _searchPage;
        private CollectionInfoPage _collectionInfoPage;
        private NftInfoPage _nftInfoPage;
        private TokenInfoPage _tokenInfoPage;
        private SearchViewAllPage _searchViewAllPage;
        private WalletDropdown _walletDropdown;

        private bool _nftInfoPageCurrencyValueRefreshTested = false;

        private bool _tokensLoaded = false;
        private bool _nftsLoaded = false;

        public void Setup(MonoBehaviour testMonobehaviour, SequenceSampleUI ui, WalletPanel walletPanel, WalletPage walletPage, LoginPanel loginPanel, TransitionPanel transitionPanel, SearchPage searchPage, CollectionInfoPage collectionInfoPage, NftInfoPage nftInfoPage, TokenInfoPage tokenInfoPage, SearchViewAllPage searchViewAllPage, WalletDropdown walletDropdown)
        {
            _testMonobehaviour = testMonobehaviour;
            _ui = ui;
            _walletPanel = walletPanel;
            _walletPage = walletPage;
            _loginPanel = loginPanel;
            _transitionPanel = transitionPanel;
            _searchPage = searchPage;
            _collectionInfoPage = collectionInfoPage;
            _nftInfoPage = nftInfoPage;
            _tokenInfoPage = tokenInfoPage;
            _searchViewAllPage = searchViewAllPage;
            _walletDropdown = walletDropdown;
        }

        public IEnumerator NavigateToWalletPageTest()
        {
            RandomNumberOfTokensToFetch = Random.Range(1, 100);
            RandomNumberOfNftsToFetch = Random.Range(1, 1000);
            AssertWeAreOnTransitionPanel();
            yield return _testMonobehaviour.StartCoroutine(TransitionToWalletPageTest());
            yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected());
        }

        public IEnumerator CloseAndReopenWalletPanelTest()
        {
            yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected());
            yield return _testMonobehaviour.StartCoroutine(CloseWalletPanelTest());
            AssertWeAreOnTransitionPanel();
            RandomNumberOfTokensToFetch = Random.Range(1, 10);
            RandomNumberOfNftsToFetch = Random.Range(1, 100);
            yield return _testMonobehaviour.StartCoroutine(TransitionToWalletPageTest());
            yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected());
        }

        public IEnumerator TestTokenInfoPage()
        {
            yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected());
            yield return _testMonobehaviour.StartCoroutine(TestInfoPages<TokenUIElement>());
        }

        public IEnumerator TestNftInfoPage()
        {
            yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected());
            yield return _testMonobehaviour.StartCoroutine(TestInfoPages<NftUIElement>());
        }

        private void AssertWeAreOnTransitionPanel()
        {
            Assert.IsFalse(_walletPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_walletPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsTrue(_transitionPanel.gameObject.activeInHierarchy);
        }

        private IEnumerator TransitionToWalletPageTest()
        {
            GameObject openWalletButtonGameObject = GameObject.Find("OpenWalletButton");
            Assert.IsNotNull(openWalletButtonGameObject);
            Button openWalletButton = openWalletButtonGameObject.GetComponent<Button>();
            Assert.IsNotNull(openWalletButton);

            _transitionPanel.TokenFetcher = new MockTokenContentFetcher(RandomNumberOfTokensToFetch, 0);
            _transitionPanel.NftFetcher = new MockNftContentFetcher(RandomNumberOfNftsToFetch, 0);
            _transitionPanel.Wallet = new MockWaaSWallet();
            Debug.Log($"Will fetch {RandomNumberOfTokensToFetch} tokens and {RandomNumberOfNftsToFetch} NFTs");
            
            openWalletButton.onClick.Invoke();
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            
            AssertWeAreOnWalletPage();
        }

        private void AssertWeAreOnWalletPage()
        {
            Assert.IsTrue(_walletPanel.gameObject.activeInHierarchy);
            Assert.IsTrue(_walletPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_transitionPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_nftInfoPage.gameObject.activeInHierarchy);
        }

        private void AssertWeAreOnNftInfoPage()
        {
            Assert.IsTrue(_walletPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_walletPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_transitionPanel.gameObject.activeInHierarchy);
            Assert.IsTrue(_nftInfoPage.gameObject.activeInHierarchy);
        }

        private IEnumerator AssertWalletPageIsAsExpected(bool isIntegrationTest = false)
        {
            AssertPanelAssumptions_WalletPage();
            AssertWeAreOnWalletPage();
            if (!isIntegrationTest)
            {
                yield return _testMonobehaviour.StartCoroutine(AssertWeLoadEnoughContent());
            }
            AssertTokensAreAboveNFTs();
            AssertWeHaveAppropriateNetworkIcons();
            AssertBrandingIsBelowContent();
            yield return _testMonobehaviour.StartCoroutine(AssertValueChangeDisplayedCorrectly());
        }

        private void AssertPanelAssumptions_WalletPage()
        {
            AssertWeAreOnWalletPage();
            Transform searchButtonTransform = _walletPanel.transform.FindAmongDecendants("SearchButton");
            Assert.IsTrue(searchButtonTransform.gameObject.activeInHierarchy);
            Transform backButtonTransform = _walletPanel.transform.FindAmongDecendants("BackButton");
            Assert.IsFalse(backButtonTransform.gameObject.activeInHierarchy);
        }

        private IEnumerator AssertWeLoadEnoughContent()
        {
            if (_walletPage.GetTokenFetcher() is MockTokenContentFetcher mockTokenFetcher)
            {
                yield return new WaitForSeconds(RandomNumberOfTokensToFetch * (float)mockTokenFetcher.DelayInMilliseconds / 1000);
            }
            else
            {
                NUnit.Framework.Assert.Fail($"Unexpected {nameof(_transitionPanel.TokenFetcher)} type. Expected {typeof(MockTokenContentFetcher)}");
            }
            
            if (_walletPage.GetNftFetcher() is MockNftContentFetcher mockNftFetcher)
            {
                yield return new WaitForSeconds(RandomNumberOfNftsToFetch * (float)mockNftFetcher.DelayInMilliseconds / 1000);
            }
            else
            {
                NUnit.Framework.Assert.Fail($"Unexpected {nameof(_transitionPanel.NftFetcher)} type. Expected {typeof(MockNftContentFetcher)}");
            }
            
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            int contentLoaded = grid.transform.childCount;
            Debug.Log($"Fetched {_walletPage.CountFungibleTokensDisplayed()} tokens and a total of {contentLoaded} content");
            Debug.Log($"Expected to fetch {RandomNumberOfTokensToFetch} tokens, {RandomNumberOfNftsToFetch} NFTs, and {RandomNumberOfTokensToFetch + RandomNumberOfNftsToFetch} total content");
            Assert.AreEqual(RandomNumberOfTokensToFetch + RandomNumberOfNftsToFetch, contentLoaded);
            Assert.AreEqual(RandomNumberOfTokensToFetch, _walletPage.CountFungibleTokensDisplayed());
        }

        private void AssertTokensAreAboveNFTs()
        {
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            int contentLoaded = grid.transform.childCount;
            bool hasSeenNFT = false;
            bool finishedSeeingTokens = false;
            bool hasSeenToken = false;

            for (int i = 0; i < contentLoaded; i++)
            {
                Transform child = grid.transform.GetChild(i);
                TokenUIElement token = child.GetComponent<TokenUIElement>();
                NftUIElement nft = child.GetComponent<NftUIElement>();
                if (nft != null && token != null)
                {
                    throw new AssertionException("Encountered an element that is both a token and an NFT",
                        "A UI element should be one or the other, not both");
                }

                if (token != null)
                {
                    hasSeenToken = true;
                    if (hasSeenNFT)
                    {
                        throw new AssertionException(
                            "Encountered a token after already encountering an NFT", "NFTs should only be found after all tokens have been found");
                    }
                }
                else if (hasSeenToken)
                {
                    finishedSeeingTokens = true;
                }
                
                if (nft != null)
                {
                    hasSeenNFT = true;
                    if (hasSeenToken && !finishedSeeingTokens)
                    {
                        throw new AssertionException("Encountered an NFT before finished seeing tokens",
                            "We should only ever see NFTs before tokens if there are no tokens to see");
                    }
                }
            }
        }

        private void AssertWeHaveAppropriateNetworkIcons()
        {
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            TokenUIElement[] tokenUIElements = grid.GetComponentsInChildren<TokenUIElement>();
            int elements = tokenUIElements.Length;
            for (int i = 0; i < elements; i++)
            {
                Transform tokenInfoGroup = tokenUIElements[i].transform.Find("TokenInfoGroup");
                Assert.IsNotNull(tokenInfoGroup);
                Image networkIconImage = tokenInfoGroup.GetComponentInChildren<Image>();
                Assert.IsNotNull(networkIconImage);
                Assert.AreEqual(tokenUIElements[i].NetworkIcons.GetIcon(tokenUIElements[i].GetNetwork()), networkIconImage.sprite);
            }
        }

        private void AssertBrandingIsBelowContent()
        {
            GameObject branding = GameObject.Find("PoweredBySequenceText");
            Assert.IsNotNull(branding);
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);

            int gridChildren = grid.transform.childCount;
            if (gridChildren == 0)
            {
                return; // We haven't fetched anything yet
            }
            RectTransform bottomContent =
                grid.transform.GetChild(grid.transform.childCount - 1).GetComponent<RectTransform>();
            RectTransform brandingTransform = branding.GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            bottomContent.GetWorldCorners(corners);
            float bottomOfContentYPosition = corners[0].y;
            brandingTransform.GetWorldCorners(corners);
            float topOfBrandingYPosition = corners[1].y;
            Assert.IsTrue(topOfBrandingYPosition < bottomOfContentYPosition);
        }

        private IEnumerator AssertValueChangeDisplayedCorrectly()
        {
            TokenUIElement token = FindObjectOfType<TokenUIElement>();
            if (token == null)
            {
                yield break; // We haven't fetched anything yet
            }
            Transform percentChange = token.transform.Find("PercentChangeText");
            Assert.IsNotNull(percentChange);
            TextMeshProUGUI percentChangeText = percentChange.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(percentChangeText);
            Transform balance = token.transform.Find("BalanceText");
            Assert.IsNotNull(balance);
            TextMeshProUGUI balanceText = balance.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(balanceText);

            uint randomBalance = (uint)Random.Range(0, 10000);
            token.RefreshWithBalance(randomBalance);
            
            AssertAppropriateColorPercentChangeText(percentChangeText);
            Assert.AreEqual("0.00%", percentChangeText.text);
            
            balanceText.text.AssertStartsWith($"{randomBalance:#,##0} ");

            yield return new WaitForSecondsRealtime(_walletPage.TimeBetweenTokenValueRefreshesInSeconds);
            
            AssertAppropriateColorPercentChangeText(percentChangeText);
            Assert.AreNotEqual("0.00%", percentChangeText.text);
        }

        private void AssertAppropriateColorPercentChangeText(TextMeshProUGUI text)
        {
            if (text.text[0] == '+')
            {
                Assert.AreEqual(Color.green, text.color);
            }else if (text.text[0] == '-')
            {
                Assert.AreEqual(Color.red, text.color);
            }
            else
            {
                Assert.AreNotEqual(Color.green, text.color);
                Assert.AreNotEqual(Color.red, text.color);
            }
        }

        private IEnumerator CloseWalletPanelTest()
        {
            GameObject closeWallet = GameObject.Find("CloseWalletButton");
            Assert.IsNotNull(closeWallet);
            Button closeWalletButton = closeWallet.GetComponent<Button>();
            Assert.IsNotNull(closeWalletButton);
            
            closeWalletButton.onClick.Invoke();
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            
            AssertWeAreOnTransitionPanel();
        }

        private IEnumerator TestInfoPages<T>() where T : WalletUIElement
        {
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            int contentLoaded = grid.transform.childCount;

            int tested = 0;
            for (int i = 0; i < contentLoaded; i++)
            {
                if (tested >= 5)
                {
                    // Finish after testing the first five to save time - if it works for the first few, it should work for the remainder
                    break;
                }
                
                Transform child = grid.transform.GetChild(i);
                T item = child.GetComponent<T>();
                if (item == null)
                {
                    continue;
                }
                
                yield return _testMonobehaviour.StartCoroutine(TestInfoPage(item));

                // Wait for tokens to load again
                if (_walletPage.GetTokenFetcher() is MockTokenContentFetcher mockTokenFetcher)
                {
                    yield return new WaitForSeconds(RandomNumberOfTokensToFetch * (float)mockTokenFetcher.DelayInMilliseconds / 1000);
                }
                else
                {
                    NUnit.Framework.Assert.Fail($"Unexpected {nameof(_transitionPanel.TokenFetcher)} type. Expected {typeof(MockTokenContentFetcher)}");
                }
                
                tested++;
            }

            yield return null;
        }

        private IEnumerator TestInfoPage(WalletUIElement element)
        {
            yield return _testMonobehaviour.StartCoroutine(NavigateToInfoPageAndAssertAssumptions(element));
            yield return _testMonobehaviour.StartCoroutine(HitUIBackButton());
            AssertPanelAssumptions_WalletPage();
        }

        private IEnumerator NavigateToInfoPageAndAssertAssumptions(WalletUIElement element)
        {
            int randomNumberOfTransactionsToFetch = Random.Range(1, 30);
            element.TransactionDetailsFetcher = new MockTransactionDetailsFetcher(randomNumberOfTransactionsToFetch, 0);
            MockTransactionDetailsFetcher fetcher = (MockTransactionDetailsFetcher)element.TransactionDetailsFetcher;
            yield return _testMonobehaviour.StartCoroutine(NavigateToInfoPageFromWalletPage(element));

            if (element is NftUIElement || element is NftWithInfoTextUIElement)
            {
                yield return _testMonobehaviour.StartCoroutine(AssertNftInfoPageIsAsExpected(element.GetNetwork(), randomNumberOfTransactionsToFetch, fetcher.DelayInMilliseconds));
            }
            else if (element is TokenUIElement)
            {
                TokenInfoPageTests tokenTests = new TokenInfoPageTests(_testMonobehaviour, _tokenInfoPage);
                yield return _testMonobehaviour.StartCoroutine(tokenTests.AssertTokenInfoPageIsAsExpected(element.GetNetwork(), randomNumberOfTransactionsToFetch, fetcher.DelayInMilliseconds));
            }
            else
            {
                NUnit.Framework.Assert.Fail($"Unexpected {typeof(WalletUIElement)} type");
            }
                
            AssertPanelAssumptions_InfoPage();
        }

        internal static IEnumerator NavigateToInfoPageFromWalletPage(WalletUIElement element)
        {
            Button button = element.GetComponent<Button>();
            Assert.IsNotNull(button);
            button.onClick.Invoke();
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
        }

        private IEnumerator HitUIBackButton()
        {
            Transform backButtonTransform = _walletPanel.transform.FindAmongDecendants("BackButton");
            Assert.IsNotNull(backButtonTransform);
            Button backButton = backButtonTransform.GetComponent<Button>();
            Assert.IsNotNull(backButton);
            backButton.onClick.Invoke();
                
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
        }

        private void AssertPanelAssumptions_InfoPage()
        {
            Transform searchButtonTransform = _walletPanel.transform.FindAmongDecendants("SearchButton");
            Assert.IsFalse(searchButtonTransform.gameObject.activeInHierarchy);
            Transform backButtonTransform = _walletPanel.transform.FindAmongDecendants("BackButton");
            Assert.IsTrue(backButtonTransform.gameObject.activeInHierarchy);
        }

        private IEnumerator AssertNftInfoPageIsAsExpected(Chain network, int randomNumberOfTransactionsToFetch, int delayInMillisecondsBetweenFetches)
        {
            Assert.IsNotNull(_nftInfoPage);

            Transform networkIcon = _nftInfoPage.transform.FindAmongDecendants("NetworkIcon");
            Assert.IsNotNull(networkIcon);
            Image networkIconImage = networkIcon.GetComponent<Image>();
            Assert.IsNotNull(networkIconImage);
            Assert.AreEqual(_nftInfoPage.GetNetworkIcon(network), networkIconImage.sprite);

            if (!_nftInfoPageCurrencyValueRefreshTested)
            {
                Transform currencyValue = _nftInfoPage.transform.FindAmongDecendants("CurrencyValueText");
                Assert.IsNotNull(currencyValue);
                TextMeshProUGUI currencyValueText = currencyValue.GetComponent<TextMeshProUGUI>();
                Assert.IsNotNull(currencyValueText);
                string currentCurrencyValue = currencyValueText.text;
                yield return new WaitForSecondsRealtime(_nftInfoPage.TimeBetweenCurrencyValueRefreshesInSeconds);
                Assert.AreNotEqual(currentCurrencyValue, currencyValueText.text);
                _nftInfoPageCurrencyValueRefreshTested = true;
            }

            Transform transactionDetailsBlockLayoutGroupTransform =
                _nftInfoPage.transform.FindAmongDecendants("TransactionDetailsBlockLayoutGroup");
            Assert.IsNotNull(transactionDetailsBlockLayoutGroupTransform);
            VerticalLayoutGroup transactionDetailsBlockLayoutGroup =
                transactionDetailsBlockLayoutGroupTransform.GetComponent<VerticalLayoutGroup>();
            Assert.IsNotNull(transactionDetailsBlockLayoutGroup);
            TransactionDetailsBlocksUITests transactionDetailsBlocksUITests =
                new TransactionDetailsBlocksUITests(_testMonobehaviour);
            yield return _testMonobehaviour.StartCoroutine(transactionDetailsBlocksUITests.AssertTransactionDetailsBlocksAreAsExpected(transactionDetailsBlockLayoutGroup, randomNumberOfTransactionsToFetch, delayInMillisecondsBetweenFetches));
        }

        public IEnumerator TestCollectionInfoPages_transitioningFromNftInfoPage()
        {
            yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected());
            
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            int contentLoaded = grid.transform.childCount;

            int tested = 0;
            for (int i = 0; i < contentLoaded; i++)
            {
                if (tested >= 3)
                {
                    // Finish after testing the first three to save time - if it works for the first few, it should work for the remainder
                    break;
                }
                
                Transform child = grid.transform.GetChild(i);
                NftUIElement item = child.GetComponent<NftUIElement>();
                if (item == null)
                {
                    continue;
                }
                
                yield return _testMonobehaviour.StartCoroutine(TestCollectionInfoPage(item));

                // Wait for tokens to load again
                if (_walletPage.GetTokenFetcher() is MockTokenContentFetcher mockTokenFetcher)
                {
                    yield return new WaitForSeconds(RandomNumberOfTokensToFetch * (float)mockTokenFetcher.DelayInMilliseconds / 1000);
                }
                else
                {
                    NUnit.Framework.Assert.Fail($"Unexpected {nameof(_transitionPanel.TokenFetcher)} type. Expected {typeof(MockTokenContentFetcher)}");
                }
                
                tested++;
            }

            yield return null;
        }
        
        private IEnumerator TestCollectionInfoPage(NftUIElement nft)
        {
            yield return _testMonobehaviour.StartCoroutine(NavigateToInfoPageAndAssertAssumptions(nft));
            CollectionInfoPageTests tests = new CollectionInfoPageTests(_testMonobehaviour, _collectionInfoPage,
                _walletPanel, _walletPage, _transitionPanel, _loginPanel, _nftInfoPage);
            yield return _testMonobehaviour.StartCoroutine(tests.NavigateToCollectionInfoPage_fromNftInfoPage());
            yield return _testMonobehaviour.StartCoroutine(tests.AssertCollectionInfoPageIsAsExpected(nft.GetCollection(), nft.NetworkIcons));
            yield return _testMonobehaviour.StartCoroutine(HitUIBackButton()); // Take us to NftInfoPage
            yield return _testMonobehaviour.StartCoroutine(HitUIBackButton()); // Take us to WalletPage
            AssertPanelAssumptions_WalletPage();
        }

        public IEnumerator TestSearchPage()
        {
            yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected());
            SearchTests searchTests = new SearchTests(_testMonobehaviour, _walletPanel, _searchPage, _collectionInfoPage, _nftInfoPage, _walletPage, _transitionPanel, _loginPanel, _tokenInfoPage, _searchViewAllPage);
            yield return _testMonobehaviour.StartCoroutine(searchTests.NavigateToSearchPageTest());
            yield return _testMonobehaviour.StartCoroutine(searchTests.NavigateToInfoPagesViaSearchElementsTest());
            yield return _testMonobehaviour.StartCoroutine(searchTests.SearchingTest());
        }

        public IEnumerator TestSearchViewAllPage()
        {
            yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected());
            SearchTests searchTests = new SearchTests(_testMonobehaviour, _walletPanel, _searchPage, _collectionInfoPage, _nftInfoPage, _walletPage, _transitionPanel, _loginPanel, _tokenInfoPage, _searchViewAllPage);
            yield return _testMonobehaviour.StartCoroutine(searchTests.NavigateToSearchPageTest());
            yield return _testMonobehaviour.StartCoroutine(searchTests.NavigateToViewAllCollectionsPageTest());
            yield return _testMonobehaviour.StartCoroutine(HitUIBackButton()); // Take us to SearchPage
            yield return _testMonobehaviour.StartCoroutine(searchTests.NavigateToViewAllTokensPageTest());
            yield return _testMonobehaviour.StartCoroutine(HitUIBackButton()); // Take us to SearchPage
            yield return _testMonobehaviour.StartCoroutine(searchTests
                .TestSearchBarEntriesRemainWhenMovingBackAndForthBetweenSearchPages());
            yield return _testMonobehaviour.StartCoroutine(searchTests.NavigateToViewAllCollectionsPageTest());
            yield return _testMonobehaviour.StartCoroutine(searchTests.ToggleViewAllPageTest());
        }

        public IEnumerator TestWalletDropdown()
        {
            AssertWeAreOnWalletPage();
            Assert.IsFalse(_walletDropdown.gameObject.activeInHierarchy);
            
            Transform topBar = _walletPanel.transform.FindAmongDecendants("TopBar");
            Assert.IsNotNull(topBar);
            TestExtensions.AssertTextWithNameHasText(topBar, "WalletAddressText", MockWaaSWallet.TestAddress.CondenseForUI());
            
            TestExtensions.ClickButtonWithName(topBar, "WalletDropdown");
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            
            AssertWeAreOnWalletPage();
            Assert.IsTrue(_walletDropdown.gameObject.activeInHierarchy);
            
            TestExtensions.AssertTextWithNameHasText(_walletDropdown.transform, "WalletAddressText", MockWaaSWallet.TestAddress.CondenseForUI());
            
            Transform copyAddressIconTransform = _walletDropdown.transform.FindAmongDecendants("CopyAddressIcon");
            Assert.IsNotNull(copyAddressIconTransform);
            Image copyAddressIcon = copyAddressIconTransform.GetComponent<Image>();
            Assert.IsNotNull(copyAddressIcon);
            Sprite copyAddressIconSprite = copyAddressIcon.sprite;
            
            TestExtensions.ClickButtonWithName(_walletDropdown.transform, "AddressLayoutGroup");
            yield return new WaitForEndOfFrame(); // Allow UI a moment to update
            Assert.AreNotEqual(copyAddressIconSprite, copyAddressIcon.sprite);
            
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            Assert.AreEqual(copyAddressIconSprite, copyAddressIcon.sprite);
            
            TestExtensions.ClickButtonWithName(_walletDropdown.transform, "CloseDropdownButton");
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            
            AssertWeAreOnWalletPage();
            Assert.IsFalse(_walletDropdown.gameObject.activeInHierarchy);
            
            TestExtensions.ClickButtonWithName(topBar, "WalletDropdown");
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            AssertWeAreOnWalletPage();
            Assert.IsTrue(_walletDropdown.gameObject.activeInHierarchy);
            
            TestExtensions.ClickButtonWithName(_walletPage.transform, "NFT(Clone)");
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in

            AssertWeAreOnNftInfoPage();
            Assert.IsFalse(_walletDropdown.gameObject.activeInHierarchy);

            yield return _testMonobehaviour.StartCoroutine(HitUIBackButton());
            AssertWeAreOnWalletPage();
            Assert.IsFalse(_walletDropdown.gameObject.activeInHierarchy);
        }
        
        
        
        public IEnumerator EndToEndTestFetchWalletContent()
        {
            yield return _testMonobehaviour.StartCoroutine(AssertContentIsFetchedAndDisplayedProperly());
            
            WalletUIElement[] elements = GetWalletElements();
            
            // Test Token info page works
            yield return _testMonobehaviour.StartCoroutine(NavigateToInfoPageAndAssertAssumptions(elements[0]));
            yield return _testMonobehaviour.StartCoroutine(HitUIBackButton());
            
            yield return _testMonobehaviour.StartCoroutine(AssertContentIsFetchedAndDisplayedProperly());
            
            elements = GetWalletElements();
            
            // Test Nft info page works
            yield return _testMonobehaviour.StartCoroutine(NavigateToInfoPageAndAssertAssumptions(elements[elements.Length - 1]));
            yield return _testMonobehaviour.StartCoroutine(HitUIBackButton());
            
            yield return _testMonobehaviour.StartCoroutine(AssertContentIsFetchedAndDisplayedProperly());
        }

        private IEnumerator AssertContentIsFetchedAndDisplayedProperly()
        {
            _tokensLoaded = false;
            _nftsLoaded = false;
            while (!_tokensLoaded || !_nftsLoaded)
            {
                yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected(isIntegrationTest: true));
            }
            yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected(isIntegrationTest: true));
        }

        private WalletUIElement[] GetWalletElements()
        {
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            int contentLoaded = grid.transform.childCount;
            Assert.IsTrue(contentLoaded > 0);
            WalletUIElement[] elements = grid.GetComponentsInChildren<WalletUIElement>();
            Assert.AreEqual(contentLoaded, elements.Length);
            return elements;
        }

        public void OnTokenFetch(FetchTokenContentResult result)
        {
            if (!result.MoreToFetch)
            {
                _tokensLoaded = true;
            }
        }

        public void OnNftFetch(FetchNftContentResult result)
        {
            if (!result.MoreToFetch)
            {
                _nftsLoaded = true;
            }
        }
    }
}