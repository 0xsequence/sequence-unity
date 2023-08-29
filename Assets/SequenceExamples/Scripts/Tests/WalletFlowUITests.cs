using System.Collections;
using Sequence.Demo;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace SequenceExamples.Scripts.Tests
{
    public class WalletFlowUITests : MonoBehaviour
    {
        private MonoBehaviour _testMonobehaviour;
            
        private SequenceUI _ui;
        private WalletPanel _walletPanel;
        private WalletPage _walletPage;
        private LoginPanel _loginPanel;
        private TransitionPanel _transitionPanel;

        private int _randomNumberOfNftsToFetch;

        public void Setup(MonoBehaviour testMonobehaviour, SequenceUI ui, WalletPanel walletPanel, WalletPage walletPage, LoginPanel loginPanel, TransitionPanel transitionPanel)
        {
            _testMonobehaviour = testMonobehaviour;
            _ui = ui;
            _walletPanel = walletPanel;
            _walletPage = walletPage;
            _loginPanel = loginPanel;
            _transitionPanel = transitionPanel;
        }

        public IEnumerator EndToEndTest()
        {
            _randomNumberOfNftsToFetch = Random.Range(0, 1000);
            yield return _testMonobehaviour.StartCoroutine(TransitionToWalletPageTest());
            yield return _testMonobehaviour.StartCoroutine(AssertWeLoadEnoughContent());
            AssertBrandingIsBelowContent();
        }

        private IEnumerator TransitionToWalletPageTest()
        {
            GameObject openWalletButtonGameObject = GameObject.Find("OpenWalletButton");
            Assert.IsNotNull(openWalletButtonGameObject);
            Button openWalletButton = openWalletButtonGameObject.GetComponent<Button>();
            Assert.IsNotNull(openWalletButton);

            _transitionPanel.NftFetcher = new MockNftContentFetcher(_randomNumberOfNftsToFetch);
            Debug.Log($"Will fetch {_randomNumberOfNftsToFetch} NFTs");
            
            openWalletButton.onClick.Invoke();
            yield return new WaitForSeconds(3f); // Wait for next page to animate in
            
            AssertWeAreOnWalletPage();
        }

        private void AssertWeAreOnWalletPage()
        {
            Assert.IsTrue(_walletPanel.gameObject.activeInHierarchy);
            Assert.IsTrue(_walletPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPanel.gameObject.activeInHierarchy);
        }

        private IEnumerator AssertWeLoadEnoughContent()
        {
            if (_transitionPanel.NftFetcher is MockNftContentFetcher mockFetcher)
            {
                yield return new WaitForSeconds(_randomNumberOfNftsToFetch * (float)mockFetcher.DelayInMilliseconds / 1000);
            }
            else
            {
                NUnit.Framework.Assert.Fail($"Unexpected {nameof(_transitionPanel.NftFetcher)} type. Expected {typeof(MockNftContentFetcher)}");
            }
            
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            int contentLoaded = grid.transform.childCount;
            Assert.AreEqual(_randomNumberOfNftsToFetch, contentLoaded);
        }

        private void AssertBrandingIsBelowContent()
        {
            GameObject branding = GameObject.Find("PoweredBySequenceText");
            Assert.IsNotNull(branding);
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);

            RectTransform bottomContent =
                grid.transform.GetChild(_randomNumberOfNftsToFetch - 1).GetComponent<RectTransform>();
            RectTransform brandingTransform = branding.GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            bottomContent.GetWorldCorners(corners);
            float bottomOfContentYPosition = corners[0].y;
            brandingTransform.GetWorldCorners(corners);
            float topOfBrandingYPosition = corners[1].y;
            Assert.IsTrue(topOfBrandingYPosition < bottomOfContentYPosition);
        }
    }
}