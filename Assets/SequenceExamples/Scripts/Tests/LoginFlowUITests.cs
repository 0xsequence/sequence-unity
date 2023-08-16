using System.Collections;
using Sequence.Demo;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SequenceExamples.Scripts.Tests
{
    public class LoginFlowUITests : MonoBehaviour
    {
        private SequenceUI _ui;
        private LoginPanel _loginPanel;
        private ConnectPage _connectPage;
        private LoginPage _loginPage;
        private MultifactorAuthenticationPage _mfaPage;
        private LoginSuccessPage _loginSuccessPage;
        
        [UnitySetUp]
        private IEnumerator LoadSceneAndWaitForAwakeAndStartAndFetchMajorElements()
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
            }

            SequenceUI.IsTesting = false;
            _ui.Start();
            yield return new WaitForSeconds(3f); // Wait a few seconds to allow for UI to animate into place
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
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator InitialExpectationsTest()
        {
            Assert.IsTrue(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_connectPage.gameObject.activeInHierarchy);
            Assert.IsTrue(_loginPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_mfaPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginSuccessPage.gameObject.activeInHierarchy);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator InitialExpectationsTest_2()
        {
            Assert.IsTrue(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_connectPage.gameObject.activeInHierarchy);
            Assert.IsTrue(_loginPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_mfaPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginSuccessPage.gameObject.activeInHierarchy);
            yield return null;
        }
    }
}