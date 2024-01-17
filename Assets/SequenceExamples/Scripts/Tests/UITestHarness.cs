using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Sequence;
using Sequence.Demo;
using Sequence.Utils;
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
        private LoginPage _loginPage;
        private MultifactorAuthenticationPage _mfaPage;
        private LoginSuccessPage _loginSuccessPage;
        private WalletPanel _walletPanel;
        private WalletPage _walletPage;
        private TransitionPanel _transitionPanel;
        private SearchPage _searchPage;
        private CollectionInfoPage _collectionInfoPage;
        private NftInfoPage _nftInfoPage;
        private TokenInfoPage _tokenInfoPage;
        private SearchViewAllPage _searchViewAllPage;
        private WalletDropdown _walletDropdown;

        public static float WaitForAnimationTime = 1.5f;

        [UnitySetUp]
        public IEnumerator LoadSceneAndWaitForAwakeAndStartAndFetchMajorElements()
        {
            SequenceSampleUI.IsTesting = true;
            SceneManager.LoadScene("Samples/Scenes/Demo");
            while (_ui == null)
            {
                yield return null; // Allow object to load
                _ui = FindObjectOfType<SequenceSampleUI>();
                _loginPanel = FindObjectOfType<LoginPanel>();
                _loginPage = FindObjectOfType<LoginPage>();
                _mfaPage = FindObjectOfType<MultifactorAuthenticationPage>();
                _loginSuccessPage = FindObjectOfType<LoginSuccessPage>();
                _walletPanel = FindObjectOfType<WalletPanel>();
                _walletPage = FindObjectOfType<WalletPage>();
                _transitionPanel = FindObjectOfType<TransitionPanel>();
                _searchPage = FindObjectOfType<SearchPage>();
                _collectionInfoPage = FindObjectOfType<CollectionInfoPage>();
                _nftInfoPage = FindObjectOfType<NftInfoPage>();
                _tokenInfoPage = FindObjectOfType<TokenInfoPage>();
                _searchViewAllPage = FindObjectOfType<SearchViewAllPage>();
                _walletDropdown = FindObjectOfType<WalletDropdown>();
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
            
            _loginFlowUITests.Setup(_testMonobehaviour, _ui, _loginPanel, _loginPage, _mfaPage, _loginSuccessPage, _walletPanel);
            _walletFlowUITests.Setup(_testMonobehaviour, _ui, _walletPanel, _walletPage, _loginPanel, _transitionPanel,
                _searchPage, _collectionInfoPage, _nftInfoPage, _tokenInfoPage, _searchViewAllPage, _walletDropdown);
        }

        [UnityTearDown]
        private IEnumerator DropMajorElements()
        {
            _ui = null;
            _loginPanel = null;
            _loginPage = null;
            _mfaPage = null;
            _loginSuccessPage = null;
            _walletPanel = null;
            _walletPage = null;
            _transitionPanel = null;
            yield return null;
        }

        [UnityTest]
        public IEnumerator LoginEmailFlowTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateTest(_loginPanel));
            yield return _testMonobehaviour.StartCoroutine(_loginFlowUITests.EndToEndEmailFlowTest());
        }

        [UnityTest]
        public IEnumerator LoginSocialFlowTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateTest(_loginPanel));
            yield return _testMonobehaviour.StartCoroutine(_loginFlowUITests.EndToEndSocialFlowTest());
        }
        
        [UnityTest]
        public IEnumerator LoginEmailFlowFailTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateTest(_loginPanel));
            yield return _testMonobehaviour.StartCoroutine(_loginFlowUITests.EmailFlowFailTest());
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
                new MockNftContentFetcher(_walletFlowUITests.RandomNumberOfNftsToFetch, 0),
                new MockWaaSWallet()));
        }

        [UnityTest]
        public IEnumerator TokenInfoPageTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateWalletPanelTest());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests.TestTokenInfoPage());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests.TestNftInfoPage());
        }

        [UnityTest]
        public IEnumerator CollectionInfoPageTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateWalletPanelTest());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests
                .TestCollectionInfoPages_transitioningFromNftInfoPage());
        }

        [UnityTest]
        public IEnumerator SearchTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateWalletPanelTest());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests.TestSearchPage());
        }

        [UnityTest]
        public IEnumerator SearchViewAllTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateWalletPanelTest());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests.TestSearchViewAllPage());
        }

        [UnityTest]
        public IEnumerator WalletDropdownTest()
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateWalletPanelTest());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests.TestWalletDropdown());
        }

        [UnityTest]
        public IEnumerator ContentFetcherEndToEndTest()
        {
            IContentFetcher contentFetcher = new ContentFetcher(new Address("0x8e3e38fe7367dd3b52d1e281e4e8400447c8d8b9"),
                EnumExtensions.GetEnumValuesAsList<Chain>().ToArray());
            ITokenContentFetcher tokenFetcher = new TokenContentFetcher(contentFetcher);
            INftContentFetcher nftFetcher = new NftContentFetcher(contentFetcher);
            tokenFetcher.OnTokenFetchSuccess += _walletFlowUITests.OnTokenFetch;
            nftFetcher.OnNftFetchSuccess += _walletFlowUITests.OnNftFetch;
            yield return _testMonobehaviour.StartCoroutine(InitiateTest(_walletPanel,
                tokenFetcher,
                nftFetcher,
                new MockWaaSWallet()));
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests.EndToEndTestFetchWalletContent());
        }
    }

    public class TestClass : MonoBehaviour
    {
        // Used to attach a monobehaviour to our test object. Unity requires a monobehaviour to attach a coroutine to - that way it can cancel the coroutine if the monobehaviour
        // gets destroyed. The test object will not be destroyed, allowing our tests to run to completion
    }
}