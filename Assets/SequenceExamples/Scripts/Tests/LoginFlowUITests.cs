using System.Collections;
using Sequence.Demo;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace SequenceExamples.Scripts.Tests
{
    public class LoginFlowUITests : MonoBehaviour
    {
        private MonoBehaviour _testMonobehaviour;
            
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

            GameObject testObject = new GameObject("TestObject");
            testObject.AddComponent<TestClass>();
            _testMonobehaviour = testObject.GetComponent<MonoBehaviour>();
            
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
        public IEnumerator IntegrationTest()
        {
            // Run all tests in one single suite to save time running test suite (otherwise, we need to reload and tear down the scene for each test
            InitialExpectationsTest();
            yield return _testMonobehaviour.StartCoroutine(TransitionToMfaPageTest());
            yield return _testMonobehaviour.StartCoroutine(TransitionToLoginSuccessPageTest());
            yield return _testMonobehaviour.StartCoroutine(GoBackToMfaPageAndVerifyPageStateTest());
            yield return _testMonobehaviour.StartCoroutine(GoBackToLoginPageAndVerifyPageStateTest());
            yield return _testMonobehaviour.StartCoroutine(NavigateToLoginSuccessPageAndDismissTest());
        }
        
        private void InitialExpectationsTest()
        {
            AssertWeAreOnLoginPage();
        }

        private void AssertWeAreOnLoginPage()
        {
            Assert.IsTrue(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_connectPage.gameObject.activeInHierarchy);
            Assert.IsTrue(_loginPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_mfaPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginSuccessPage.gameObject.activeInHierarchy);
        }
        
        private IEnumerator TransitionToMfaPageTest()
        {
            yield return _testMonobehaviour.StartCoroutine(TransitionToMfaPage("validEmail@valid.com"));
        }

        private IEnumerator TransitionToMfaPage(string email)
        {
            GameObject buttonGameObject = GameObject.Find("LoginButton");
            Assert.IsNotNull(buttonGameObject);
            Button button = buttonGameObject.GetComponent<Button>();
            Assert.IsNotNull(button);
            GameObject emailGameObject = GameObject.Find("EmailField");
            Assert.IsNotNull(emailGameObject);
            TMP_InputField emailInputField = emailGameObject.GetComponent<TMP_InputField>();
            Assert.IsNotNull(emailInputField);

            emailInputField.text = email;
            yield return null;
            
            button.onClick.Invoke();
            yield return new WaitForSeconds(3f); // Wait for next page to animate in

            AssertWeAreOnMfaPage();
            
            GameObject textGameObject = GameObject.Find("EnterCodeText");
            Assert.IsNotNull(textGameObject);
            TextMeshProUGUI text = textGameObject.GetComponent<TextMeshProUGUI>();
            Assert.AreEqual($"Enter the code sent to\n<b>{email}</b>", text.text);
        }

        private void AssertWeAreOnMfaPage()
        {
            Assert.IsTrue(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_connectPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPage.gameObject.activeInHierarchy);
            Assert.IsTrue(_mfaPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginSuccessPage.gameObject.activeInHierarchy);
        }

        private IEnumerator TransitionToLoginSuccessPageTest()
        {
            yield return _testMonobehaviour.StartCoroutine(TransitionToLoginSuccessPage("1234567890"));
        }

        private IEnumerator TransitionToLoginSuccessPage(string code)
        {
            TMP_InputField MFACodeField = FetchMfaCodeFieldAndAssertAssumptions();
            GameObject buttonGameObject = GameObject.Find("ContinueButton");
            Assert.IsNotNull(buttonGameObject);
            Button button = buttonGameObject.GetComponent<Button>();
            Assert.IsNotNull(button);
            TextMeshProUGUI[] boxesTexts = FetchMfaBoxesAndAssertAssumptions();

            MFACodeField.text = code;
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(code[0].ToString(), boxesTexts[0].text);
            Assert.AreEqual(code[1].ToString(), boxesTexts[1].text);
            Assert.AreEqual(code[2].ToString(), boxesTexts[2].text);
            Assert.AreEqual(code[3].ToString(), boxesTexts[3].text);
            Assert.AreEqual(code[4].ToString(), boxesTexts[4].text);
            Assert.AreEqual(code[5].ToString(), boxesTexts[5].text);
            
            button.onClick.Invoke();
            yield return new WaitForSeconds(10f); // Wait for next page to animate in

            AssertWeAreOnLoginSuccessPage();
        }

        private TMP_InputField FetchMfaCodeFieldAndAssertAssumptions()
        {
            GameObject MfaCodeGameObject = GameObject.Find("MFACodeField");
            Assert.IsNotNull(MfaCodeGameObject);
            TMP_InputField MfaCodeField = MfaCodeGameObject.GetComponent<TMP_InputField>();
            Assert.IsNotNull(MfaCodeField);
            Assert.AreEqual("", MfaCodeField.text);
            return MfaCodeField;
        }

        private TextMeshProUGUI[] FetchMfaBoxesAndAssertAssumptions()
        {
            GameObject boxesParent = GameObject.Find("Boxes");
            Assert.IsNotNull(boxesParent);
            Image[] boxes = boxesParent.GetComponentsInChildren<Image>();
            Assert.IsNotNull(boxes);
            Assert.AreEqual(6, boxes.Length);
            TextMeshProUGUI[] boxesTexts = new TextMeshProUGUI[boxes.Length];
            for (int i = 0; i < boxes.Length; i++)
            {
                boxesTexts[i] = boxes[i].GetComponentInChildren<TextMeshProUGUI>();
                Assert.IsNotNull(boxesTexts[i]);
                Assert.AreEqual("", boxesTexts[i].text);
            }
            return boxesTexts;
        }

        private void AssertWeAreOnLoginSuccessPage()
        {
            Assert.IsTrue(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_connectPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_mfaPage.gameObject.activeInHierarchy);
            Assert.IsTrue(_loginSuccessPage.gameObject.activeInHierarchy);
        }

        private IEnumerator GoBackToMfaPageAndVerifyPageStateTest()
        {
            GameObject backGameObject = GameObject.Find("BackButton");
            Assert.IsNotNull(backGameObject);
            Button backButton = backGameObject.GetComponent<Button>();
            Assert.IsNotNull(backButton);
            
            backButton.onClick.Invoke();
            yield return new WaitForSeconds(3f); // Wait for next page to animate in
            
            AssertWeAreOnMfaPage();
            
            FetchMfaCodeFieldAndAssertAssumptions();
            FetchMfaBoxesAndAssertAssumptions();
            
            GameObject textGameObject = GameObject.Find("EnterCodeText");
            Assert.IsNotNull(textGameObject);
            TextMeshProUGUI text = textGameObject.GetComponent<TextMeshProUGUI>();
            Assert.AreEqual("Enter the code sent to\n<b>validEmail@valid.com</b>", text.text);
        }

        private IEnumerator GoBackToLoginPageAndVerifyPageStateTest()
        {
            GameObject backGameObject = GameObject.Find("BackButton");
            Assert.IsNotNull(backGameObject);
            Button backButton = backGameObject.GetComponent<Button>();
            Assert.IsNotNull(backButton);
            
            backButton.onClick.Invoke();
            yield return new WaitForSeconds(3f); // Wait for next page to animate in
            
            AssertWeAreOnLoginPage();
            
            GameObject emailGameObject = GameObject.Find("EmailField");
            Assert.IsNotNull(emailGameObject);
            TMP_InputField emailInputField = emailGameObject.GetComponent<TMP_InputField>();
            Assert.IsNotNull(emailInputField);
            Assert.AreEqual("validEmail@valid.com", emailInputField.text);
            
            backGameObject = GameObject.Find("BackButton");
            Assert.IsNull(backGameObject);
        }

        private IEnumerator NavigateToLoginSuccessPageAndDismissTest()
        {
            yield return _testMonobehaviour.StartCoroutine(TransitionToMfaPage("newValidEmail@valid.com"));
            yield return _testMonobehaviour.StartCoroutine(TransitionToLoginSuccessPage("0987654321"));
            
            GameObject dismissGameObject = GameObject.Find("DismissButton");
            Assert.IsNotNull(dismissGameObject);
            Button dismissButton = dismissGameObject.GetComponent<Button>();
            Assert.IsNotNull(dismissButton);
            
            dismissButton.onClick.Invoke();
            yield return new WaitForSeconds(3f); // Wait for next page to animate in
            
            AssertLoginPanelIsClosed();
        }

        private void AssertLoginPanelIsClosed()
        {
            Assert.IsFalse(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_connectPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_mfaPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginSuccessPage.gameObject.activeInHierarchy);
        }
    }

    public class TestClass : MonoBehaviour
    {
        // Used to attach a monobehaviour to our test object. Unity requires a monobehaviour to attach a coroutine to - that way it can cancel the coroutine if the monobehaviour
        // gets destroyed. The test object will not be destroyed, allowing our tests to run to completion
    }
}