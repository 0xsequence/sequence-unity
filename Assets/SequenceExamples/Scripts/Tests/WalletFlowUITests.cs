using System;
using System.Collections;
using Sequence.Demo;
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
        private MonoBehaviour _testMonobehaviour;
            
        private SequenceUI _ui;
        private WalletPanel _walletPanel;
        private WalletPage _walletPage;
        private LoginPanel _loginPanel;
        private TransitionPanel _transitionPanel;

        private int _randomNumberOfTokensToFetch;
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
            _randomNumberOfTokensToFetch = Random.Range(0, 100);
            _randomNumberOfNftsToFetch = Random.Range(0, 1000);
            yield return _testMonobehaviour.StartCoroutine(TransitionToWalletPageTest());
            yield return _testMonobehaviour.StartCoroutine(AssertWeLoadEnoughContent());
            AssertBrandingIsBelowContent();
            yield return _testMonobehaviour.StartCoroutine(AssertValueChangeDisplayedCorrectly());
        }

        private IEnumerator TransitionToWalletPageTest()
        {
            GameObject openWalletButtonGameObject = GameObject.Find("OpenWalletButton");
            Assert.IsNotNull(openWalletButtonGameObject);
            Button openWalletButton = openWalletButtonGameObject.GetComponent<Button>();
            Assert.IsNotNull(openWalletButton);

            _transitionPanel.TokenFetcher = new MockTokenContentFetcher(_randomNumberOfTokensToFetch);
            _transitionPanel.NftFetcher = new MockNftContentFetcher(_randomNumberOfNftsToFetch);
            Debug.Log($"Will fetch {_randomNumberOfTokensToFetch} tokens and {_randomNumberOfNftsToFetch} NFTs");
            
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
            if (_transitionPanel.TokenFetcher is MockTokenContentFetcher mockTokenFetcher)
            {
                yield return new WaitForSeconds(_randomNumberOfNftsToFetch * (float)mockTokenFetcher.DelayInMilliseconds / 1000);
            }
            else
            {
                NUnit.Framework.Assert.Fail($"Unexpected {nameof(_transitionPanel.TokenFetcher)} type. Expected {typeof(MockTokenContentFetcher)}");
            }
            
            if (_transitionPanel.NftFetcher is MockNftContentFetcher mockNftFetcher)
            {
                yield return new WaitForSeconds(_randomNumberOfNftsToFetch * (float)mockNftFetcher.DelayInMilliseconds / 1000);
            }
            else
            {
                NUnit.Framework.Assert.Fail($"Unexpected {nameof(_transitionPanel.NftFetcher)} type. Expected {typeof(MockNftContentFetcher)}");
            }
            
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            int contentLoaded = grid.transform.childCount;
            Assert.AreEqual(_randomNumberOfTokensToFetch + _randomNumberOfNftsToFetch, contentLoaded);
            Assert.AreEqual(_randomNumberOfTokensToFetch, _walletPage.CountFungibleTokensDisplayed());
        }

        private void AssertBrandingIsBelowContent()
        {
            GameObject branding = GameObject.Find("PoweredBySequenceText");
            Assert.IsNotNull(branding);
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);

            RectTransform bottomContent =
                grid.transform.GetChild(_randomNumberOfTokensToFetch + _randomNumberOfNftsToFetch - 1).GetComponent<RectTransform>();
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

            yield return new WaitForSecondsRealtime(_walletPage.TimeBetweenTokenValueRefreshesInSeconds);
            
            AssertAppropriateColorPercentChangeText(percentChangeText);
            
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
    }
}