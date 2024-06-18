using System.Collections;
using Sequence.Authentication;
using Sequence.Demo;
using SequenceExamples.Scripts.Tests.Utils;
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
            
        private SequenceSampleUI _ui;
        private LoginPanel _loginPanel;
        private LoginPage _loginPage;
        private MultifactorAuthenticationPage _mfaPage;
        private LoginSuccessPage _loginSuccessPage;
        private WalletPanel _walletPanel;
        
        private static readonly WaitForSeconds WaitForAnimationTime = new WaitForSeconds(UITestHarness.WaitForAnimationTime);

        public void Setup(MonoBehaviour testMonobehaviour, SequenceSampleUI ui, LoginPanel loginPanel,  LoginPage loginPage,
            MultifactorAuthenticationPage mfaPage, LoginSuccessPage loginSuccessPage, WalletPanel walletPanel)
        {
            _testMonobehaviour = testMonobehaviour;
            _ui = ui;
            _loginPanel = loginPanel;
            _loginPage = loginPage;
            _loginPage.NotifyUserIfTheyAreLoggingInWithADifferentAccountFromLastTime = false;
            _mfaPage = mfaPage;
            _loginSuccessPage = loginSuccessPage;
            _walletPanel = walletPanel;
            
            _loginPanel.SetupLoginHandler(new MockLogin());
        }

        public IEnumerator EndToEndEmailFlowTest()
        {
            // Run all tests in one single suite to save time running test suite (otherwise, we need to reload and tear down the scene for each test
            InitialExpectationsTest();
            yield return _testMonobehaviour.StartCoroutine(TransitionToMfaPageTest());
            yield return _testMonobehaviour.StartCoroutine(TransitionToLoginSuccessPageTest());
            yield return _testMonobehaviour.StartCoroutine(GoBackToMfaPageAndVerifyPageStateTest());
            yield return _testMonobehaviour.StartCoroutine(GoBackToLoginPageAndVerifyPageStateTest("validEmail@valid.com"));
            yield return _testMonobehaviour.StartCoroutine(NavigateToLoginSuccessPageAndDismissTest());
        }

        public IEnumerator EndToEndSocialFlowTest()
        {
            // Run all tests in one single suite to save time running test suite (otherwise, we need to reload and tear down the scene for each test
            InitialExpectationsTest();
            string expectedEmail = TestExtensions.GetTextFromInputFieldWithName(_loginPage.transform, "EmailField");
            foreach (string provider in new[] {"Google", "Discord", "Facebook", "Apple"})
            {
                yield return _testMonobehaviour.StartCoroutine(NavigateToLoginSuccessPageViaSocialLoginTest(provider));
                yield return _testMonobehaviour.StartCoroutine(GoBackToLoginPageAndVerifyPageStateTest(expectedEmail));
            }
            
            yield return _testMonobehaviour.StartCoroutine(NavigateToLoginSuccessPageViaSocialLoginAndDismissTest("Google"));
        }
        
        private void InitialExpectationsTest()
        {
            AssertWeAreOnLoginPage();
        }

        private void AssertWeAreOnLoginPage()
        {
            Assert.IsTrue(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsTrue(_loginPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_mfaPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginSuccessPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_walletPanel.gameObject.activeInHierarchy);
        }
        
        public IEnumerator TransitionToMfaPageTest()
        {
            yield return _testMonobehaviour.StartCoroutine(TransitionToMfaPage("validEmail@valid.com"));
        }

        private IEnumerator TransitionToMfaPage(string email)
        {
            yield return _testMonobehaviour.StartCoroutine(InitiateTransitionToMfaPage(email));

            AssertWeAreOnMfaPage();
            
            GameObject textGameObject = GameObject.Find("EnterCodeText");
            Assert.IsNotNull(textGameObject);
            TextMeshProUGUI text = textGameObject.GetComponent<TextMeshProUGUI>();
            Assert.AreEqual($"Enter the code sent to\n<b>{email}</b>", text.text);
        }

        private IEnumerator InitiateTransitionToMfaPage(string email)
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
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
        }

        private void AssertWeAreOnMfaPage()
        {
            Assert.IsTrue(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPage.gameObject.activeInHierarchy);
            Assert.IsTrue(_mfaPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginSuccessPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_walletPanel.gameObject.activeInHierarchy);
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
        
        private IEnumerator NavigateToLoginSuccessPageViaSocialLoginAndDismissTest(string providerName)
        {
            yield return NavigateToLoginSuccessPageViaSocialLoginTest(providerName);
            yield return DismissTest();
        }

        private IEnumerator NavigateToLoginSuccessPageViaSocialLoginTest(string providerName)
        {
            TestExtensions.ClickButtonWithName(_loginPage.transform, $"{providerName}SignInButton");
            yield return WaitForAnimationTime;
            
            AssertWeAreOnLoginSuccessPage();
        }
        
        private TMP_InputField FetchMfaCodeFieldAndAssertAssumptions()
        {
            TMP_InputField MfaCodeField = FetchMfaCodeField();
            Assert.AreEqual("", MfaCodeField.text);
            return MfaCodeField;
        }

        private TMP_InputField FetchMfaCodeField()
        {
            GameObject MfaCodeGameObject = GameObject.Find("MFACodeField");
            Assert.IsNotNull(MfaCodeGameObject);
            TMP_InputField MfaCodeField = MfaCodeGameObject.GetComponent<TMP_InputField>();
            Assert.IsNotNull(MfaCodeField);
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
            Assert.IsFalse(_loginPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_mfaPage.gameObject.activeInHierarchy);
            Assert.IsTrue(_loginSuccessPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_walletPanel.gameObject.activeInHierarchy);
        }

        private IEnumerator GoBackToMfaPageAndVerifyPageStateTest()
        {
            GameObject backGameObject = GameObject.Find("BackButton");
            Assert.IsNotNull(backGameObject);
            Button backButton = backGameObject.GetComponent<Button>();
            Assert.IsNotNull(backButton);
            
            backButton.onClick.Invoke();
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            
            AssertWeAreOnMfaPage();
            
            FetchMfaCodeFieldAndAssertAssumptions();
            FetchMfaBoxesAndAssertAssumptions();
            
            GameObject textGameObject = GameObject.Find("EnterCodeText");
            Assert.IsNotNull(textGameObject);
            TextMeshProUGUI text = textGameObject.GetComponent<TextMeshProUGUI>();
            Assert.AreEqual("Enter the code sent to\n<b>validEmail@valid.com</b>", text.text);
        }

        private IEnumerator GoBackToLoginPageAndVerifyPageStateTest(string expectedEmail)
        {
            GameObject backGameObject = GameObject.Find("BackButton");
            Assert.IsNotNull(backGameObject);
            Button backButton = backGameObject.GetComponent<Button>();
            Assert.IsNotNull(backButton);
            
            backButton.onClick.Invoke();
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            
            AssertWeAreOnLoginPage();
            
            GameObject emailGameObject = GameObject.Find("EmailField");
            Assert.IsNotNull(emailGameObject);
            TMP_InputField emailInputField = emailGameObject.GetComponent<TMP_InputField>();
            Assert.IsNotNull(emailInputField);
            Assert.AreEqual(expectedEmail, emailInputField.text);
            
            backGameObject = GameObject.Find("BackButton");
            Assert.IsNull(backGameObject);
        }

        public IEnumerator NavigateToLoginSuccessPageAndDismissTest()
        {
            yield return _testMonobehaviour.StartCoroutine(TransitionToMfaPage("newValidEmail@valid.com"));
            yield return _testMonobehaviour.StartCoroutine(TransitionToLoginSuccessPage("0987654321"));
            yield return _testMonobehaviour.StartCoroutine(DismissTest());
        }

        private IEnumerator DismissTest()
        {
            GameObject dismissGameObject = GameObject.Find("DismissButton");
            Assert.IsNotNull(dismissGameObject);
            Button dismissButton = dismissGameObject.GetComponent<Button>();
            Assert.IsNotNull(dismissButton);
            
            dismissButton.onClick.Invoke();
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            
            AssertLoginPanelIsClosed();
        }

        private void AssertLoginPanelIsClosed()
        {
            Assert.IsFalse(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_mfaPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginSuccessPage.gameObject.activeInHierarchy);
        }

        public IEnumerator EmailFlowFailTest()
        {
            AssertWeAreOnLoginPage();
            TestExtensions.AssertTextWithNameHasText(_loginPage.transform, "ErrorText", "");
            yield return _testMonobehaviour.StartCoroutine(InitiateTransitionToMfaPage("invalidEmail"));
            AssertWeAreOnLoginPage();
            TestExtensions.AssertTextWithNameHasText(_loginPage.transform, "ErrorText", "Invalid email: invalidEmail");
            LogAssert.Expect(LogType.Error, "Failed to send MFA email to invalidEmail with error: Invalid email: invalidEmail");

            yield return _testMonobehaviour.StartCoroutine(InitiateTransitionToMfaPage("failSend@fakeDomain.com"));
            AssertWeAreOnLoginPage();
            TestExtensions.AssertTextWithNameHasText(_loginPage.transform, "ErrorText", "Failed to send email to failSend@fakeDomain.com");
            LogAssert.Expect(LogType.Error, "Failed to send MFA email to failSend@fakeDomain.com with error: Failed to send email to failSend@fakeDomain.com");

            yield return _testMonobehaviour.StartCoroutine(TransitionToMfaPage("failLogin@noReason.net"));
            
            AssertWeAreOnMfaPage();
            TMP_InputField MfaCodeField = FetchMfaCodeField();
            MfaCodeField.text = "invalidCode";
            TestExtensions.ClickButtonWithName(_mfaPage.transform, "ContinueButton");
            yield return WaitForAnimationTime;
            TestExtensions.AssertTextWithNameHasText(_mfaPage.transform, "ErrorText", "Login failed because of invalid code");
            LogAssert.Expect(LogType.Error, "Failed login: Login failed because of invalid code");
            
            MfaCodeField.text = "12345";
            TestExtensions.ClickButtonWithName(_mfaPage.transform, "ContinueButton");
            yield return WaitForAnimationTime;
            TestExtensions.AssertTextWithNameHasText(_mfaPage.transform, "ErrorText", "Login failed because of invalid code");
            LogAssert.Expect(LogType.Error, "Failed login: Login failed because of invalid code");
            
            MfaCodeField.text = "12e456";
            TestExtensions.ClickButtonWithName(_mfaPage.transform, "ContinueButton");
            yield return WaitForAnimationTime;
            TestExtensions.AssertTextWithNameHasText(_mfaPage.transform, "ErrorText", "Login failed because of invalid code");
            LogAssert.Expect(LogType.Error, "Failed login: Login failed because of invalid code");
            
            MfaCodeField.text = "123456";
            TestExtensions.ClickButtonWithName(_mfaPage.transform, "ContinueButton");
            yield return WaitForAnimationTime;
            TestExtensions.AssertTextWithNameHasText(_mfaPage.transform, "ErrorText", "Login failed for some reason");
            LogAssert.Expect(LogType.Error, "Failed login: Login failed for some reason");
        }

        public IEnumerator LoadingScreenTest()
        {
            AssertWeAreOnLoginPage();
            MockLoginCustomEventTiming loginHandler = new MockLoginCustomEventTiming();
            _loginPanel.SetupLoginHandler(loginHandler);
            CheckLoadingScreenPresence(false);
            
            TestExtensions.ClickButtonWithName(_loginPage.transform, "LoginButton");
            CheckLoadingScreenPresence(true);
            yield return WaitForAnimationTime;
            CheckLoadingScreenPresence(true);
            loginHandler.FireOnMFAEmailFailedToSendEvent();
            LogAssert.Expect(LogType.Error, "Failed to send MFA email to  with error: ");
            yield return null;
            CheckLoadingScreenPresence(false);

            TestExtensions.ClickButtonWithName(_loginPage.transform, "GoogleSignInButton");
            CheckLoadingScreenPresence(true);
            yield return WaitForAnimationTime;
            CheckLoadingScreenPresence(true);
            loginHandler.FireOnMFAEmailFailedToSendEvent();
            LogAssert.Expect(LogType.Error, "Failed to send MFA email to  with error: ");
            yield return null;
            CheckLoadingScreenPresence(false);

            TestExtensions.ClickButtonWithName(_loginPage.transform, "DiscordSignInButton");
            CheckLoadingScreenPresence(true);
            yield return WaitForAnimationTime;
            CheckLoadingScreenPresence(true);
            loginHandler.FireOnLoginFailedEvent();
            LogAssert.Expect(LogType.Error, "Failed to sign in to WaaS API with error: ");
            LogAssert.Expect(LogType.Error, "Failed login: ");
            yield return null;
            CheckLoadingScreenPresence(false);

            TestExtensions.ClickButtonWithName(_loginPage.transform, "FacebookSignInButton");
            CheckLoadingScreenPresence(true);
            yield return WaitForAnimationTime;
            CheckLoadingScreenPresence(true);
            loginHandler.FireOnLoginFailedEvent();
            LogAssert.Expect(LogType.Error, "Failed to sign in to WaaS API with error: ");
            LogAssert.Expect(LogType.Error, "Failed login: ");
            yield return null;
            CheckLoadingScreenPresence(false);

            TestExtensions.ClickButtonWithName(_loginPage.transform, "AppleSignInButton");
            CheckLoadingScreenPresence(true);
            yield return WaitForAnimationTime;
            CheckLoadingScreenPresence(true);
            loginHandler.FireOnLoginFailedEvent();
            LogAssert.Expect(LogType.Error, "Failed to sign in to WaaS API with error: ");
            LogAssert.Expect(LogType.Error, "Failed login: ");
            yield return null;
            CheckLoadingScreenPresence(false);
            
            TestExtensions.ClickButtonWithName(_loginPage.transform, "LoginButton");
            CheckLoadingScreenPresence(true);
            yield return WaitForAnimationTime;
            CheckLoadingScreenPresence(true);
            loginHandler.FireOnMFAEmailSentEvent();
            yield return null;
            CheckLoadingScreenPresence(false);
            
            AssertWeAreOnMfaPage();
            
            TestExtensions.ClickButtonWithName(_mfaPage.transform, "ContinueButton");
            CheckLoadingScreenPresence(true);
            yield return WaitForAnimationTime;
            CheckLoadingScreenPresence(true);
            loginHandler.FireOnLoginSuccessEvent();
            yield return null;
            CheckLoadingScreenPresence(false);
            
            AssertWeAreOnLoginSuccessPage();
        }

        private void CheckLoadingScreenPresence(bool isPresent)
        {
            LoadingScreen loadingScreen = FindObjectOfType<LoadingScreen>();
            if (isPresent)
            {
                Assert.IsNotNull(loadingScreen);
            }
            else
            {
                Assert.IsNull(loadingScreen);
            }
        }
    }
}