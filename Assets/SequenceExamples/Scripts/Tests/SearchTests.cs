using System;
using System.Collections;
using Sequence.Demo;
using SequenceExamples.Scripts.Tests.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace SequenceExamples.Scripts.Tests
{
    public class SearchTests
    {
        private MonoBehaviour _testMonoBehaviour;
        private WalletPanel _panel;
        private SearchPage _searchPage;

        public SearchTests(MonoBehaviour testMonoBehaviour, WalletPanel panel, SearchPage searchPage)
        {
            _testMonoBehaviour = testMonoBehaviour;
            _panel = panel;
            _searchPage = searchPage;
        }

        public IEnumerator NavigateToSearchPageTest()
        {
            TestExtensions.ClickButtonWithName(_panel.transform, "SearchButton");
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            
            AssertSearchPageIsAsExpected();
        }

        private void AssertSearchPageIsAsExpected()
        {
            Transform elementLayoutGroup = _panel.transform.FindAmongDecendants("ElementLayoutGroup");
            Assert.IsNotNull(elementLayoutGroup);
            int otherElements = 3; // ViewAllCollectionsButton, Spacer, ViewAllTokensButton
            Assert.IsTrue(_searchPage.MaxSearchElementsDisplayed >= elementLayoutGroup.childCount - otherElements);
            
            TestExtensions.AssertTextWithNameHasText(_panel.transform, "CollectionCountText", $"Collections ({_panel.GetCollections().Length})");
            TestExtensions.AssertTextWithNameHasText(_panel.transform, "TokenCountText", $"Coins ({_panel.GetFetchedTokenElements().Length})");

            AssertSearchElementsAreAssembledCorrectly(elementLayoutGroup, otherElements);
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
            
            Transform elementLayoutGroup = _panel.transform.FindAmongDecendants("ElementLayoutGroup");
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
                    yield return null;
                }
                else if (searchElement.Searchable is TokenElement token)
                {
                    yield return null;
                }
                else
                {
                    throw new SystemException(
                        $"Encountered unexpected type of {nameof(ISearchable)}, given {searchElement.Searchable.GetType()}");
                }
            }
        }
    }
}