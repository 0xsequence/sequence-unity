using System.Collections;
using Sequence.Demo;
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
            
        private SequenceUI _ui;
        private LoginPanel _loginPanel;
        private ConnectPage _connectPage;
        private LoginPage _loginPage;
        private MultifactorAuthenticationPage _mfaPage;
        private LoginSuccessPage _loginSuccessPage;
        private WalletPanel _walletPanel;
        private WalletPage _walletPage;

        [UnitySetUp]
        public IEnumerator LoadSceneAndWaitForAwakeAndStartAndFetchMajorElements()
        {
            SequenceUI.IsTesting = true;
            SceneManager.LoadScene("SequenceExamples/Scenes/Demo");
            while (_ui == null)
            {
                yield return null; // Allow object to load
                _ui = FindObjectOfType<SequenceUI>();
                _loginPanel = FindObjectOfType<LoginPanel>();
                _connectPage = FindObjectOfType<ConnectPage>();
                _loginPage = FindObjectOfType<LoginPage>();
                _mfaPage = FindObjectOfType<MultifactorAuthenticationPage>();
                _loginSuccessPage = FindObjectOfType<LoginSuccessPage>();
                _walletPanel = FindObjectOfType<WalletPanel>();
                _walletPage = FindObjectOfType<WalletPage>();
            }

            GameObject testObject = new GameObject("TestObject");
            _testMonobehaviour = testObject.AddComponent<TestClass>();
            _loginFlowUITests = testObject.AddComponent<LoginFlowUITests>();
            _walletFlowUITests = testObject.AddComponent<WalletFlowUITests>();
            
            SequenceUI.IsTesting = false;
            _ui.Start();
            yield return new WaitForSeconds(3f); // Wait a few seconds to allow for UI to animate into place
            
            _loginFlowUITests.Setup(_ui, _loginPanel, _connectPage, _loginPage, _mfaPage, _loginSuccessPage, _walletPanel, _testMonobehaviour);
            _walletFlowUITests.Setup(_ui, _walletPanel, _walletPage, _loginPanel, _testMonobehaviour);
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
            yield return null;
        }

        [UnityTest]
        public IEnumerator LoginFlowTest()
        {
            yield return _testMonobehaviour.StartCoroutine(_loginFlowUITests.EndToEndTest());
        }

        [UnityTest]
        public IEnumerator WalletFlowTest()
        {
            yield return
                _testMonobehaviour.StartCoroutine(_loginFlowUITests.NavigateToLoginSuccessPageAndDismissTest());
            yield return _testMonobehaviour.StartCoroutine(_walletFlowUITests.EndToEndTest());
        }
    }
}