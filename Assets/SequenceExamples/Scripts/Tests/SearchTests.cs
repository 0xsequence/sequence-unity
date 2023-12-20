using System;
using System.Collections;
using Sequence.Demo;
using SequenceExamples.Scripts.Tests.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
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
        private SearchViewAllPage _searchViewAllPage;
        int _otherElementsCount = 3; // ViewAllCollectionsButton, Spacer, ViewAllTokensButton
        
        public SearchTests(MonoBehaviour testMonoBehaviour, WalletPanel walletPanel, SearchPage searchPage, CollectionInfoPage collectionInfoPage, NftInfoPage nftInfoPage, WalletPage walletPage, TransitionPanel transitionPanel, LoginPanel loginPanel, TokenInfoPage tokenInfoPage, SearchViewAllPage searchViewAllPage)
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
            _searchViewAllPage = searchViewAllPage;
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
            
            TestExtensions.AssertTextWithNameHasText(_walletPanel.transform, "CollectionCountText", $"Collections ({_searchPage.GetQuerier().GetNumberOfCollectionsMatchingCriteria()})");
            TestExtensions.AssertTextWithNameHasText(_walletPanel.transform, "TokenCountText", $"Coins ({_searchPage.GetQuerier().GetNumberOfTokensMatchingCriteria()})");

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

        public IEnumerator NavigateToViewAllCollectionsPageTest()
        {
            AssertSearchPageIsAsExpected();
            
            TestExtensions.ClickButtonWithName(_searchPage.transform, "ViewAllCollectionsButton");
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in

            yield return _testMonoBehaviour.StartCoroutine(AssertViewAllCollectionsPageIsAsExpected());
        }

        private IEnumerator AssertViewAllCollectionsPageIsAsExpected()
        {
            yield return _testMonoBehaviour.StartCoroutine(
                AssertViewAllPageIsAsExpected<SearchableCollection>(_searchPage.GetQuerier()
                    .GetNumberOfCollectionsMatchingCriteria()));
        }

        private IEnumerator AssertViewAllPageIsAsExpected<ExpectedSearchableType>(
            int expectedSearchablesMatchingCriteria)
        {
            AssertCorrectToggleText();

            Transform elementLayoutGroup = _searchViewAllPage.transform.FindAmongDecendants("SearchableContent");
            Assert.IsNotNull(elementLayoutGroup);
            Assert.AreEqual(expectedSearchablesMatchingCriteria, elementLayoutGroup.childCount);

            AssertAllDisplayedSearchablesAreOfTypeT<ExpectedSearchableType>(elementLayoutGroup);

            int randomInfoPageToTest = Random.Range(0, elementLayoutGroup.childCount);
            yield return _testMonoBehaviour.StartCoroutine(
                NavigateToInfoPageAndBackTest(elementLayoutGroup.GetChild(randomInfoPageToTest)));
        }

        private void AssertCorrectToggleText()
        {
            TestExtensions.AssertTextWithNameHasText(_searchViewAllPage.transform, "CollectionToggleText", 
                $"Collections ({_searchPage.GetQuerier().GetNumberOfCollectionsMatchingCriteria()})");
            TestExtensions.AssertTextWithNameHasText(_searchViewAllPage.transform, "TokenToggleText",
                $"Coins ({_searchPage.GetQuerier().GetNumberOfTokensMatchingCriteria()})");
        }

        private void AssertAllDisplayedSearchablesAreOfTypeT<T>(Transform layoutGroup)
        {
            int childCount = layoutGroup.childCount;
            for (int i = 0; i < childCount; i++)
            {
                SearchElement element = layoutGroup.GetChild(i).GetComponent<SearchElement>();
                Assert.IsNotNull(element);
                Assert.IsTrue(element.Searchable is T);
            }
        }

        public IEnumerator NavigateToViewAllTokensPageTest()
        {
            AssertSearchPageIsAsExpected();
            
            TestExtensions.ClickButtonWithName(_searchPage.transform, "ViewAllTokensButton");
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in

            yield return _testMonoBehaviour.StartCoroutine(AssertViewAllTokensPageIsAsExpected());
        }

        private IEnumerator AssertViewAllTokensPageIsAsExpected()
        {
            yield return _testMonoBehaviour.StartCoroutine(
                AssertViewAllPageIsAsExpected<TokenElement>(_searchPage.GetQuerier()
                    .GetNumberOfTokensMatchingCriteria()));
        }

        public IEnumerator TestSearchBarEntriesRemainWhenMovingBackAndForthBetweenSearchPages()
        {
            AssertSearchPageIsAsExpected();
            
            TMP_InputField mainSearchBar = _searchPage.GetComponentInChildren<TMP_InputField>();
            Assert.IsNotNull(mainSearchBar);
            
            mainSearchBar.text = "s";
            TestExtensions.ClickButtonWithName(_searchPage.transform, "ViewAllCollectionsButton");
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in

            TMP_InputField viewAllPageSearchBar = _searchViewAllPage.GetComponentInChildren<TMP_InputField>();
            Assert.IsNotNull(viewAllPageSearchBar);
            Assert.AreEqual("s", viewAllPageSearchBar.text);

            int expectedSearchablesMatchingCriteria = _searchPage.GetQuerier().GetNumberOfCollectionsMatchingCriteria();
            Transform elementLayoutGroup = _searchViewAllPage.transform.FindAmongDecendants("SearchableContent");
            Assert.IsNotNull(elementLayoutGroup);
            Assert.AreEqual(expectedSearchablesMatchingCriteria, elementLayoutGroup.childCount);

            if (expectedSearchablesMatchingCriteria > 0)
            {
                int randomInfoPageToTest = Random.Range(0, expectedSearchablesMatchingCriteria);
                yield return _testMonoBehaviour.StartCoroutine(
                    NavigateToInfoPageAndBackTest(elementLayoutGroup.GetChild(randomInfoPageToTest)));
            }
            
            Assert.AreEqual("s", viewAllPageSearchBar.text);
            viewAllPageSearchBar.text = "m";
            yield return _testMonoBehaviour.StartCoroutine(HitUIBackButton());

            Assert.AreEqual("m", mainSearchBar.text);
            mainSearchBar.text = "se";
            TestExtensions.ClickButtonWithName(_searchPage.transform, "ViewAllCollectionsButton");
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in

            Assert.AreEqual("se", viewAllPageSearchBar.text);
            expectedSearchablesMatchingCriteria = _searchPage.GetQuerier().GetNumberOfCollectionsMatchingCriteria();
            Assert.AreEqual(expectedSearchablesMatchingCriteria, elementLayoutGroup.childCount);
            
            yield return _testMonoBehaviour.StartCoroutine(HitUIBackButton());
            Assert.AreEqual("se", mainSearchBar.text);
        }

        public IEnumerator ToggleViewAllPageTest()
        {
            Transform collectionToggleTransform = _searchViewAllPage.transform.FindAmongDecendants("CollectionToggle");
            Assert.IsNotNull(collectionToggleTransform);
            Toggle collectionToggle = collectionToggleTransform.GetComponent<Toggle>();
            Assert.IsNotNull(collectionToggle);
            Transform tokenToggleTransform = _searchViewAllPage.transform.FindAmongDecendants("TokenToggle");
            Assert.IsNotNull(tokenToggleTransform);
            Toggle tokenToggle = tokenToggleTransform.GetComponent<Toggle>();
            Assert.IsNotNull(tokenToggle);
            Transform elementLayoutGroup = _searchViewAllPage.transform.FindAmongDecendants("SearchableContent");
            Assert.IsNotNull(elementLayoutGroup);
            
            Assert.IsTrue(collectionToggle.isOn);
            Assert.IsFalse(tokenToggle.isOn);
            AssertAllDisplayedSearchablesAreOfTypeT<SearchableCollection>(elementLayoutGroup);
            
            tokenToggle.OnPointerClick(new PointerEventData(EventSystem.current));
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Allow UI to update
            
            Assert.IsFalse(collectionToggle.isOn);
            Assert.IsTrue(tokenToggle.isOn);
            
            collectionToggle.OnPointerClick(new PointerEventData(EventSystem.current));
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Allow UI to update
            
            Assert.IsTrue(collectionToggle.isOn);
            Assert.IsFalse(tokenToggle.isOn);

            yield return _testMonoBehaviour.StartCoroutine(HitUIBackButton());
            yield return _testMonoBehaviour.StartCoroutine(NavigateToViewAllTokensPageTest());
            
            Assert.IsFalse(collectionToggle.isOn);
            Assert.IsTrue(tokenToggle.isOn);
            AssertAllDisplayedSearchablesAreOfTypeT<TokenElement>(elementLayoutGroup);
            
            collectionToggle.OnPointerClick(new PointerEventData(EventSystem.current));
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Allow UI to update
            
            Assert.IsTrue(collectionToggle.isOn);
            Assert.IsFalse(tokenToggle.isOn);
            
            tokenToggle.OnPointerClick(new PointerEventData(EventSystem.current));
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Allow UI to update
            
            Assert.IsFalse(collectionToggle.isOn);
            Assert.IsTrue(tokenToggle.isOn);

            yield return _testMonoBehaviour.StartCoroutine(HitUIBackButton());
            yield return _testMonoBehaviour.StartCoroutine(NavigateToViewAllCollectionsPageTest());
            
            Assert.IsTrue(collectionToggle.isOn);
            Assert.IsFalse(tokenToggle.isOn);
            AssertAllDisplayedSearchablesAreOfTypeT<SearchableCollection>(elementLayoutGroup);
        }
    }
}