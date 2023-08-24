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

        public void Setup(SequenceUI ui, WalletPanel walletPanel, WalletPage walletPage, LoginPanel loginPanel,
            MonoBehaviour testMonobehaviour)
        {
            _ui = ui;
            _walletPanel = walletPanel;
            _walletPage = walletPage;
            _loginPanel = loginPanel;
            _testMonobehaviour = testMonobehaviour;
        }

        public IEnumerator EndToEndTest()
        {
            yield return _testMonobehaviour.StartCoroutine(TransitionToWalletPageTest());
        }

        private IEnumerator TransitionToWalletPageTest()
        {
            GameObject openWalletButtonGameObject = GameObject.Find("OpenWalletButton");
            Assert.IsNotNull(openWalletButtonGameObject);
            Button openWalletButton = openWalletButtonGameObject.GetComponent<Button>();
            Assert.IsNotNull(openWalletButton);
            
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
    }
}