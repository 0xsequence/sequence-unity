using System.Collections;
using System.Collections.Generic;
using Sequence;
using Sequence.Demo;
using Sequence.Demo.ScriptableObjects;
using SequenceExamples.Scripts.Tests.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SequenceExamples.Scripts.Tests
{
    public class CollectionInfoPageTests
    {
        private MonoBehaviour _testMonobehaviour;
        private CollectionInfoPage _collectionInfoPage;
        private WalletPanel _walletPanel;
        private WalletPage _walletPage;
        private TransitionPanel _transitionPanel;
        private LoginPanel _loginPanel;
        private NftInfoPage _nftInfoPage;

        public CollectionInfoPageTests(MonoBehaviour testMonobehaviour, CollectionInfoPage collectionInfoPage, WalletPanel walletPanel, WalletPage walletPage, TransitionPanel transitionPanel, LoginPanel loginPanel, NftInfoPage nftInfoPage)
        {
            _testMonobehaviour = testMonobehaviour;
            _collectionInfoPage = collectionInfoPage;
            _walletPanel = walletPanel;
            _walletPage = walletPage;
            _transitionPanel = transitionPanel;
            _loginPanel = loginPanel;
            _nftInfoPage = nftInfoPage;
        }

        internal IEnumerator NavigateToCollectionInfoPage_fromNftInfoPage()
        {
            GameObject collectionGroup = GameObject.Find("CollectionNameLayoutGroup");
            Assert.IsNotNull(collectionGroup);
            Button collectionGroupNavigationButton = collectionGroup.GetComponent<Button>();
            Assert.IsNotNull(collectionGroupNavigationButton);
            collectionGroupNavigationButton.onClick.Invoke();
                
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
        }

        internal IEnumerator AssertCollectionInfoPageIsAsExpected(CollectionInfo collection, NetworkIcons networkIcons)
        {
            Assert.IsNotNull(_collectionInfoPage);
            Assert.IsTrue(_collectionInfoPage.gameObject.activeInHierarchy);
            
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
                yield return _testMonobehaviour.StartCoroutine(WalletFlowUITests.NavigateToInfoPageFromWalletPage(element));
                AssertNftInfoPageIsAssembledCorrectly(expected[i]);
                yield return _testMonobehaviour.StartCoroutine(HitUIBackButton());
            }
        }

        private void AssertNftInfoPageIsAssembledCorrectly(NftElement nft)
        {
            Assert.IsTrue(_walletPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_walletPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_transitionPanel.gameObject.activeInHierarchy);
            
            Assert.IsTrue(_nftInfoPage.gameObject.activeInHierarchy);
            
            AssertPanelAssumptions_InfoPage();
            
            Assert.AreEqual(nft, _nftInfoPage.GetNftElement());
        }
        
        private IEnumerator HitUIBackButton()
        {
            TestExtensions.ClickButtonWithName(_walletPanel.transform, "BackButton");
                
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
        }
        
        private void AssertPanelAssumptions_InfoPage()
        {
            Transform searchButtonTransform = _walletPanel.transform.FindAmongDecendants("SearchButton");
            Assert.IsFalse(searchButtonTransform.gameObject.activeInHierarchy);
            Transform backButtonTransform = _walletPanel.transform.FindAmongDecendants("BackButton");
            Assert.IsTrue(backButtonTransform.gameObject.activeInHierarchy);
        }
    }
}