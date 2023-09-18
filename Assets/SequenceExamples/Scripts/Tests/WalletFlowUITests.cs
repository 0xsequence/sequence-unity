using System;
using System.Collections;
using System.Collections.Generic;
using Sequence;
using Sequence.Demo;
using Sequence.Demo.ScriptableObjects;
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
        private CollectionInfoPage _collectionInfoPage;
        
        public void Setup(MonoBehaviour testMonobehaviour, SequenceSampleUI ui, WalletPanel walletPanel, WalletPage walletPage, LoginPanel loginPanel, TransitionPanel transitionPanel)
        {
            _testMonobehaviour = testMonobehaviour;
            _ui = ui;
            _walletPanel = walletPanel;
            _walletPage = walletPage;
            _loginPanel = loginPanel;
            _transitionPanel = transitionPanel;
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
        }

        private IEnumerator AssertWalletPageIsAsExpected()
        {
            AssertPanelAssumptions_WalletPage();
            AssertWeAreOnWalletPage();
            yield return _testMonobehaviour.StartCoroutine(AssertWeLoadEnoughContent());
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
            if (_transitionPanel.TokenFetcher is MockTokenContentFetcher mockTokenFetcher)
            {
                yield return new WaitForSeconds(RandomNumberOfTokensToFetch * (float)mockTokenFetcher.DelayInMilliseconds / 1000);
            }
            else
            {
                NUnit.Framework.Assert.Fail($"Unexpected {nameof(_transitionPanel.TokenFetcher)} type. Expected {typeof(MockTokenContentFetcher)}");
            }
            
            if (_transitionPanel.NftFetcher is MockNftContentFetcher mockNftFetcher)
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

            RectTransform bottomContent =
                grid.transform.GetChild(RandomNumberOfTokensToFetch + RandomNumberOfNftsToFetch - 1).GetComponent<RectTransform>();
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
            Assert.IsNotNull(token);
            Transform percentChange = token.transform.Find("PercentChangeText");
            Assert.IsNotNull(percentChange);
            TextMeshProUGUI percentChangeText = percentChange.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(percentChangeText);
            Transform balance = token.transform.Find("BalanceText");
            Assert.IsNotNull(balance);
            TextMeshProUGUI balanceText = balance.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(balanceText);
            
            token.RefreshWithBalance(5);
            
            AssertAppropriateColorPercentChangeText(percentChangeText);
            Assert.AreEqual("0.00%", percentChangeText.text);
            
            balanceText.text.AssertStartsWith("5 ");

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
                if (_transitionPanel.TokenFetcher is MockTokenContentFetcher mockTokenFetcher)
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
            Button button = element.GetComponent<Button>();
            Assert.IsNotNull(button);
                
            button.onClick.Invoke();
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in

            if (element is NftUIElement || element is NftWithInfoTextUIElement)
            {
                yield return _testMonobehaviour.StartCoroutine(AssertNftInfoPageIsAsExpected(element.GetNetwork(), randomNumberOfTransactionsToFetch, fetcher.DelayInMilliseconds));
            }
            else if (element is TokenUIElement)
            {
                yield return _testMonobehaviour.StartCoroutine(AssertTokenInfoPageIsAsExpected(element.GetNetwork(), randomNumberOfTransactionsToFetch, fetcher.DelayInMilliseconds));
            }
            else
            {
                NUnit.Framework.Assert.Fail($"Unexpected {typeof(WalletUIElement)} type");
            }
                
            AssertPanelAssumptions_InfoPage();
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

        private IEnumerator AssertTokenInfoPageIsAsExpected(Chain network, int randomNumberOfTransactionsToFetch, int delayInMillisecondsBetweenFetches)
        {
            TokenInfoPage tokenInfo = FindObjectOfType<TokenInfoPage>();
            Assert.IsNotNull(tokenInfo);

            Transform networkBanner = tokenInfo.transform.Find("NetworkBanner");
            Assert.IsNotNull(networkBanner);
            Transform networkIcon = networkBanner.transform.Find("NetworkIcon");
            Assert.IsNotNull(networkIcon);
            Image networkIconImage = networkIcon.GetComponent<Image>();
            Assert.IsNotNull(networkIconImage);
            Assert.AreEqual(tokenInfo.GetNetworkIcon(network), networkIconImage.sprite);
            Transform networkName = networkBanner.transform.Find("NetworkName");
            Assert.IsNotNull(networkName);
            TextMeshProUGUI networkNameText = networkName.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(networkNameText);
            Assert.AreEqual(ChainNames.NameOf[network], networkNameText.text);

            Transform currencyValue = tokenInfo.transform.Find("CurrencyValueText");
            Assert.IsNotNull(currencyValue);
            TextMeshProUGUI currencyValueText = currencyValue.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(currencyValueText);
            string currentCurrencyValue = currencyValueText.text;
            yield return new WaitForSecondsRealtime(tokenInfo.TimeBetweenCurrencyValueRefreshesInSeconds);
            Assert.AreNotEqual(currentCurrencyValue, currencyValueText.text);

            GameObject transactionScrollView = GameObject.Find("TransactionsScrollView");
            Assert.IsNotNull(transactionScrollView);
            VerticalLayoutGroup transactionVerticalLayoutGroup =
                transactionScrollView.GetComponentInChildren<VerticalLayoutGroup>();
            Assert.IsNotNull(transactionVerticalLayoutGroup);
            TransactionDetailsBlocksUITests transactionDetailsBlocksUITests =
                new TransactionDetailsBlocksUITests(_testMonobehaviour);
            yield return _testMonobehaviour.StartCoroutine(transactionDetailsBlocksUITests.AssertTransactionDetailsBlocksAreAsExpected(transactionVerticalLayoutGroup, randomNumberOfTransactionsToFetch, delayInMillisecondsBetweenFetches));
        }

        private IEnumerator AssertNftInfoPageIsAsExpected(Chain network, int randomNumberOfTransactionsToFetch, int delayInMillisecondsBetweenFetches)
        {
            NftInfoPage nftInfoPage = FindObjectOfType<NftInfoPage>();
            Assert.IsNotNull(nftInfoPage);

            Transform networkIcon = nftInfoPage.transform.FindAmongDecendants("NetworkIcon");
            Assert.IsNotNull(networkIcon);
            Image networkIconImage = networkIcon.GetComponent<Image>();
            Assert.IsNotNull(networkIconImage);
            Assert.AreEqual(nftInfoPage.GetNetworkIcon(network), networkIconImage.sprite);

            Transform currencyValue = nftInfoPage.transform.FindAmongDecendants("CurrencyValueText");
            Assert.IsNotNull(currencyValue);
            TextMeshProUGUI currencyValueText = currencyValue.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(currencyValueText);
            string currentCurrencyValue = currencyValueText.text;
            yield return new WaitForSecondsRealtime(nftInfoPage.TimeBetweenCurrencyValueRefreshesInSeconds);
            Assert.AreNotEqual(currentCurrencyValue, currencyValueText.text);

            Transform transactionDetailsBlockLayoutGroupTransform =
                nftInfoPage.transform.FindAmongDecendants("TransactionDetailsBlockLayoutGroup");
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
                if (_transitionPanel.TokenFetcher is MockTokenContentFetcher mockTokenFetcher)
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
            yield return _testMonobehaviour.StartCoroutine(NavigateToCollectionInfoPage_fromNftInfoPage());
            yield return _testMonobehaviour.StartCoroutine(AssertCollectionInfoPageIsAsExpected(nft.GetCollection(), nft.NetworkIcons));
            yield return _testMonobehaviour.StartCoroutine(HitUIBackButton()); // Take us to NftInfoPage
            yield return _testMonobehaviour.StartCoroutine(HitUIBackButton()); // Take us to WalletPage
            AssertPanelAssumptions_WalletPage();
        }

        private IEnumerator NavigateToCollectionInfoPage_fromNftInfoPage()
        {
            GameObject collectionGroup = GameObject.Find("CollectionNameLayoutGroup");
            Assert.IsNotNull(collectionGroup);
            Button collectionGroupNavigationButton = collectionGroup.GetComponent<Button>();
            Assert.IsNotNull(collectionGroupNavigationButton);
            collectionGroupNavigationButton.onClick.Invoke();
                
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
        }

        private IEnumerator AssertCollectionInfoPageIsAsExpected(CollectionInfo collection, NetworkIcons networkIcons)
        {
            _collectionInfoPage = FindObjectOfType<CollectionInfoPage>();
            Assert.IsNotNull(_collectionInfoPage);
            
            TestExtensions.AssertImageWithNameHasSprite(_collectionInfoPage.transform, "CollectionIcon", collection.IconSprite);
            TestExtensions.AssertTextWithNameHasText(_collectionInfoPage.transform, "CollectionNameText", collection.Name);
            TestExtensions.AssertImageWithNameHasSprite(_collectionInfoPage.transform, "NetworkIcon", networkIcons.GetIcon(collection.Network));
            TestExtensions.AssertTextWithNameHasText(_collectionInfoPage.transform, "NetworkName", ChainNames.NameOf[collection.Network]);
            TestExtensions.AssertTextWithNameHasText(_collectionInfoPage.transform, "UniqueCollectiblesOwnedText", $"{_walletPanel.GetNftsFromCollection(collection).Count} Unique Collectibles");
            TestExtensions.AssertTextWithNameHasText(_collectionInfoPage.transform, "TotalOwnedText", $"Owned ({NftElement.CalculateTotalNftsOwned(_walletPanel.GetNftsFromCollection(collection))})");

            yield return _testMonobehaviour.StartCoroutine(AssertNftWithInfoTextsAreAsExpected(collection));
        }

        private IEnumerator AssertNftWithInfoTextsAreAsExpected(CollectionInfo collection)
        {
            Transform ownedElementsGroup = _collectionInfoPage.transform.FindAmongDecendants("OwnedElementsGroup");
            Assert.IsNotNull(ownedElementsGroup);
            
            List<NftElement> expected = _walletPanel.GetNftsFromCollection(collection);
            int childCount = ownedElementsGroup.childCount;
            Assert.AreEqual(expected.Count, childCount);
            
            for (int i = 0; i < childCount; i++)
            {
                TestExtensions.AssertImageWithNameHasSprite(ownedElementsGroup.GetChild(i), "NFT", expected[i].TokenIconSprite);
                TestExtensions.AssertTextWithNameHasText(ownedElementsGroup.GetChild(i), "NftNameText", expected[i].TokenName);
                TestExtensions.AssertTextWithNameHasText(ownedElementsGroup.GetChild(i), "NumberOwnedText", $"{expected[i].Balance} Owned");
                
                if (i >= 3)
                {
                    continue; // Can skip remainder of test when i >= 3 to save time
                }

                NftWithInfoTextUIElement element = ownedElementsGroup.GetChild(i).GetComponent<NftWithInfoTextUIElement>();
                Assert.IsNotNull(element);
                yield return _testMonobehaviour.StartCoroutine(NavigateToInfoPageAndAssertAssumptions(element));
                yield return _testMonobehaviour.StartCoroutine(HitUIBackButton());
            }
        }
    }
}