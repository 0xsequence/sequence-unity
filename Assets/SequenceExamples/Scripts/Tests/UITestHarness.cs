using System.Collections;
using NUnit.Framework;
using Sequence.Demo;
using SequenceExamples.Scripts.Tests.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SequenceExamples.Scripts.Tests
{
    public class UITestHarness : MonoBehaviour
    {
        private MonoBehaviour _testMonobehaviour;
        private LoginFlowUITests _loginFlowUITests;
        private WalletFlowUITests _walletFlowUITests;
            
        private SequenceSampleUI _ui;
        private LoginPanel _loginPanel;
        private ConnectPage _connectPage;
        private LoginPage _loginPage;
        private MultifactorAuthenticationPage _mfaPage;
        private LoginSuccessPage _loginSuccessPage;
        private WalletPanel _walletPanel;
        private WalletPage _walletPage;
        private TransitionPanel _transitionPanel;

        public static float WaitForAnimationTime = 1.5f;

        [UnitySetUp]
        public IEnumerator LoadSceneAndWaitForAwakeAndStartAndFetchMajorElements()
        {
            SequenceSampleUI.IsTesting = true;
            SceneManager.LoadScene("SequenceExamples/Scenes/Demo");
            while (_ui == null)
            {
                yield return null; // Allow object to load
                _ui = FindObjectOfType<SequenceSampleUI>();
                _loginPanel = FindObjectOfType<LoginPanel>();
                _connectPage = FindObjectOfType<ConnectPage>();
                _loginPage = FindObjectOfType<LoginPage>();
                _mfaPage = FindObjectOfType<MultifactorAuthenticationPage>();
                _loginSuccessPage = FindObjectOfType<LoginSuccessPage>();
                _walletPanel = FindObjectOfType<WalletPanel>();
                _walletPage = FindObjectOfType<WalletPage>();
                _transitionPanel = FindObjectOfType<TransitionPanel>();
            }

            GameObject testObject = new GameObject("TestObject");
            _testMonobehaviour = testObject.AddComponent<TestClass>();
            _loginFlowUITests = testObject.AddComponent<LoginFlowUITests>();
            _walletFlowUITests = testObject.AddComponent<WalletFlowUITests>();
            MockDelayOverrider mockDelayOverrider = testObject.AddComponent<MockDelayOverrider>();
            mockDelayOverrider.OverrideAnimationTimes(WaitForAnimationTime / 1000);
        }

        private IEnumerator InitiateTest(UIPanel initialPanel, params object[] args)
        {
            SequenceSampleUI.InitialPanel = initialPanel;
            SequenceSampleUI.InitialPanelOpenArgs = args;
            SequenceSampleUI.IsTesting = false; // Allows test to run
            _ui.Start();
            yield return new WaitForSeconds(WaitForAnimationTime); // Wait a few seconds to allow for UI to animate into place
            
            _loginFlowUITests.Setup(_testMonobehaviour, _ui, _loginPanel, _connectPage, _loginPage, _mfaPage, _loginSuccessPage, _walletPanel);
            _walletFlowUITests.Setup(_testMonobehaviour, _ui, _walletPanel, _walletPage, _loginPanel, _transitionPanel);
        }

        [UnityTearDown]
        private IEnumerator DropMajorElements()
        {
            _ui = null;
            _loginPanel = null;
            _connectPage = null;
            _loginPage = null;
            _mfaPage = null;
            _loginSuccessPage = null;
            _walletPanel = null;
            _walletPage = null;
            _transitionPanel = null;
            yield return null;
        }

        [UnityTest]
        public IEnumerator LoginFlowTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateTest(_loginPanel));
            yield return _testMonobehaviour.StartCoroutine(_loginFlowUITests.EndToEndTest());
        }

        [UnityTest]
        public IEnumerator NavigateToWalletPageFromLoginTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateTest(_loginPanel));
            yield return
                _testMonobehaviour.StartCoroutine(_loginFlowUITests.NavigateToLoginSuccessPageAndDismissTest());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests.NavigateToWalletPageTest());
        }

        [UnityTest]
        public IEnumerator WalletPanelCloseAndReopenTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateWalletPanelTest());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests.CloseAndReopenWalletPanelTest());
        }

        private IEnumerator InitiateWalletPanelTest()
        {
            _walletFlowUITests.RandomNumberOfTokensToFetch = Random.Range(1, 10);
            _walletFlowUITests.RandomNumberOfNftsToFetch = Random.Range(1, 100);
            yield return _testMonobehaviour.StartCoroutine(InitiateTest(_walletPanel,
                new MockTokenContentFetcher(_walletFlowUITests.RandomNumberOfTokensToFetch, 0),
                new MockNftContentFetcher(_walletFlowUITests.RandomNumberOfNftsToFetch, 0)));
        }

        [UnityTest]
        public IEnumerator TokenInfoPageTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateWalletPanelTest());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests.TestTokenInfoPage());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests.TestNftInfoPage());
        }

        [UnityTest]
        [Timeout(1500000)]
        public IEnumerator CollectionInfoPageTest_navigatingThroughNftInfoPages()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateWalletPanelTest());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests
                .TestCollectionInfoPages_transitioningFromNftInfoPage());
        }
    }
}