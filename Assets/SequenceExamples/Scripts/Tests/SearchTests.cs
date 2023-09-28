using System;
using System.Collections;
using Sequence.Demo;
using SequenceExamples.Scripts.Tests.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SequenceExamples.Scripts.Tests
{
    public class SearchTests
    {
        private MonoBehaviour _testMonoBehaviour;
        private WalletPanel _walletPanel;
        private SearchPage _searchPage;
        private CollectionInfoPage _collectionInfoPage;
        private NftInfoPage _nftInfoPage;
        private WalletPage _walletPage;
        private TransitionPanel _transitionPanel;
        private LoginPanel _loginPanel;
        private TokenInfoPage _tokenInfoPage;
        int _otherElementsCount = 3; // ViewAllCollectionsButton, Spacer, ViewAllTokensButton
        
        public SearchTests(MonoBehaviour testMonoBehaviour, WalletPanel walletPanel, SearchPage searchPage, CollectionInfoPage collectionInfoPage, NftInfoPage nftInfoPage, WalletPage walletPage, TransitionPanel transitionPanel, LoginPanel loginPanel, TokenInfoPage tokenInfoPage)
        {
            _testMonoBehaviour = testMonoBehaviour;
            _walletPanel = walletPanel;
            _searchPage = searchPage;
            _collectionInfoPage = collectionInfoPage;
            _nftInfoPage = nftInfoPage;
            _walletPage = walletPage;
            _transitionPanel = transitionPanel;
            _loginPanel = loginPanel;
            _tokenInfoPage = tokenInfoPage;
        }

        public IEnumerator NavigateToSearchPageTest()
        {
            TestExtensions.ClickButtonWithName(_walletPanel.transform, "SearchButton");
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            
            AssertSearchPageIsAsExpected();
        }

        private void AssertSearchPageIsAsExpected()
        {
            Transform elementLayoutGroup = _walletPanel.transform.FindAmongDecendants("ElementLayoutGroup");
            Assert.IsNotNull(elementLayoutGroup);
            Assert.IsTrue(_searchPage.MaxSearchElementsDisplayed >= elementLayoutGroup.childCount - _otherElementsCount);
            
            TestExtensions.AssertTextWithNameHasText(_walletPanel.transform, "CollectionCountText", $"Collections ({_walletPanel.GetCollections().Length})");
            TestExtensions.AssertTextWithNameHasText(_walletPanel.transform, "TokenCountText", $"Coins ({_walletPanel.GetFetchedTokenElements().Length})");

            AssertSearchElementsAreAssembledCorrectly(elementLayoutGroup, _otherElementsCount);
            AssertSearchElementsAreOrderedCorrectly(elementLayoutGroup);
        }

        private void AssertSearchElementsAreAssembledCorrectly(Transform elementLayoutGroup, int otherElements)
        {
            int elements = elementLayoutGroup.childCount;
            int skipped = 0;
            for (int i = 0; i < elements; i++)
            {
                Transform current = elementLayoutGroup.GetChild(i);
                SearchElement searchElement = current.GetComponent<SearchElement>();
                if (searchElement == null)
                {
                    skipped++;
                    Assert.IsTrue(skipped <= otherElements);
                    continue;
                }
                TestExtensions.AssertImageWithNameHasSprite(searchElement.transform, "Icon", searchElement.Searchable.GetIcon());
                TestExtensions.AssertTextWithNameHasText(searchElement.transform, "Name", searchElement.Searchable.GetName());
                TestExtensions.AssertImageWithNameHasSprite(searchElement.transform, "NetworkIcon", searchElement.GetNetworkIcon());
                TestExtensions.AssertTextWithNameHasText(searchElement.transform, "NumberOwnedText", $"{searchElement.Searchable.GetNumberOwned()} >");
            }
        }

        private void AssertSearchElementsAreOrderedCorrectly(Transform elementLayoutGroup)
        {
            int elements = elementLayoutGroup.childCount;
            bool enteredTokenRegion = false;
            for (int i = 0; i < elements; i++)
            {
                Transform current = elementLayoutGroup.GetChild(i);
                SearchElement searchElement = current.GetComponent<SearchElement>();
                if (searchElement == null)
                {
                    if (current.name == "ViewAllTokensButton")
                    {
                        enteredTokenRegion = true;
                    }
                    continue;
                }

                if (enteredTokenRegion)
                {
                    Assert.IsTrue(searchElement.Searchable is not SearchableCollection);
                }
            }
        }

        public IEnumerator NavigateToInfoPagesViaSearchElementsTest()
        {
            AssertSearchPageIsAsExpected();
            
            Transform elementLayoutGroup = _walletPanel.transform.FindAmongDecendants("ElementLayoutGroup");
            Assert.IsNotNull(elementLayoutGroup);
            int childCount = elementLayoutGroup.childCount;
            for (int i = 0; i < childCount; i++)
            {
                yield return _testMonoBehaviour.StartCoroutine(NavigateToInfoPageAndBackTest(elementLayoutGroup.GetChild(i)));
            }
        }

        private IEnumerator NavigateToInfoPageAndBackTest(Transform element)
        {
            SearchElement searchElement = element.GetComponent<SearchElement>();
            if (searchElement != null)
            {
                if (searchElement.Searchable is SearchableCollection collection)
                {
                    CollectionInfoPageTests collectionTests = new CollectionInfoPageTests(_testMonoBehaviour, _collectionInfoPage,
                        _walletPanel, _walletPage, _transitionPanel, _loginPanel, _nftInfoPage);
                    yield return _testMonoBehaviour.StartCoroutine(
                        collectionTests.NavigateToCollectionInfoPage_fromSearchPage(searchElement));
                    yield return _testMonoBehaviour.StartCoroutine(
                        collectionTests.AssertCollectionInfoPageIsAsExpected(collection.GetCollection(), searchElement.NetworkIcons));
                }
                else if (searchElement.Searchable is TokenElement token)
                {
                    TokenInfoPageTests tokenTests = new TokenInfoPageTests(_testMonoBehaviour, _tokenInfoPage);
                    int randomNumberOfTransactionsToFetch = Random.Range(1, 10);
                    yield return _testMonoBehaviour.StartCoroutine(NavigateToTokenInfoPage(searchElement, randomNumberOfTransactionsToFetch));
                    yield return _testMonoBehaviour.StartCoroutine(
                        tokenTests.AssertTokenInfoPageIsAsExpected(token.Network, randomNumberOfTransactionsToFetch,
                            0));
                }
                else
                {
                    throw new SystemException(
                        $"Encountered unexpected type of {nameof(ISearchable)}, given {searchElement.Searchable.GetType()}");
                }
                yield return _testMonoBehaviour.StartCoroutine(HitUIBackButton());
            }
        }
        
        private IEnumerator HitUIBackButton()
        {
            TestExtensions.ClickButtonWithName(_walletPanel.transform, "BackButton");
                
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
        }

        private IEnumerator NavigateToTokenInfoPage(SearchElement searchElement, int randomNumberOfTransactionsToFetch)
        {
            searchElement.TransactionDetailsFetcher =
                new MockTransactionDetailsFetcher(randomNumberOfTransactionsToFetch, 0);
            
            Button button = searchElement.GetComponent<Button>();
            Assert.IsNotNull(button);
            button.onClick.Invoke();
                
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
        }

        public IEnumerator SearchingTest()
        {
            AssertAppropriateAmountTexts();
            
            TMP_InputField searchBar = _searchPage.GetComponentInChildren<TMP_InputField>();
            Assert.IsNotNull(searchBar);
            Transform elementLayoutGroup = _searchPage.transform.FindAmongDecendants("ElementLayoutGroup");
            Assert.IsNotNull(elementLayoutGroup);
            int children = GetEnabledChildCount(elementLayoutGroup);
            
            string nothingMatchesWith = "@!$^&GD&SBGsuwdgfyuasb*(#QRNidnsf89w";
            searchBar.text = nothingMatchesWith;
            yield return new WaitForEndOfFrame();
            Assert.AreNotEqual(children, GetEnabledChildCount(elementLayoutGroup));
            Assert.AreEqual(_otherElementsCount, GetEnabledChildCount(elementLayoutGroup));
            Assert.AreEqual(0, _searchPage.GetQuerier().GetNumberOfCollectionsMatchingCriteria() + _searchPage.GetQuerier().GetNumberOfTokensMatchingCriteria());
            AssertAppropriateAmountTexts();

            searchBar.text = "";
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(children, GetEnabledChildCount(elementLayoutGroup));
            AssertAppropriateAmountTexts();

            string[] potentialStartingValues = new[] { "s", "m", "a" };
            int childrenNow = 0;
            for (int i = 0; i < potentialStartingValues.Length; i++)
            {
                searchBar.text = potentialStartingValues[i];
                yield return new WaitForEndOfFrame();
                childrenNow = GetEnabledChildCount(elementLayoutGroup);
                if (childrenNow > 0 && childrenNow < children)
                {
                    break;
                }
            }
            Assert.IsTrue(childrenNow > 0 && childrenNow < children);
            AssertAppropriateAmountTexts();

            yield return _testMonoBehaviour.StartCoroutine(NavigateToInfoPageAndBackTest(elementLayoutGroup.GetChild(0)));
            
            Assert.AreEqual(childrenNow, GetEnabledChildCount(elementLayoutGroup));
            AssertAppropriateAmountTexts();
        }

        private int GetEnabledChildCount(Transform t)
        {
            int children = t.childCount;
            int enabled = 0;
            for (int i = 0; i < children; i++)
            {
                if (t.GetChild(i).gameObject.activeInHierarchy)
                {
                    enabled++;
                }
            }

            return enabled;
        }

        private void AssertAppropriateAmountTexts()
        {
            TestExtensions.AssertTextWithNameHasText(_searchPage.transform, "CollectionCountText", $"Collections ({_searchPage.GetQuerier().GetNumberOfCollectionsMatchingCriteria()})");
            TestExtensions.AssertTextWithNameHasText(_searchPage.transform, "TokenCountText", $"Coins ({_searchPage.GetQuerier().GetNumberOfTokensMatchingCriteria()})");
        }
    }
}