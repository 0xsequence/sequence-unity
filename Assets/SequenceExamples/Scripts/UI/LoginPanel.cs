using System.Collections;
using Sequence.Authentication;
using UnityEngine;

namespace Sequence.Demo
{
    public class LoginPanel : UIPanel
    {
        private TransitionPanel _transitionPanel;
        private ConnectPage _connectPage;
        private LoginPage _loginPage;
        private MultifactorAuthenticationPage _mfaPage;
        private LoginSuccessPage _loginSuccessPage;
        protected override void Awake()
        {
            base.Awake();
            _transitionPanel = FindObjectOfType<TransitionPanel>();
            
            _connectPage = GetComponentInChildren<ConnectPage>();
            
            ILogin loginHandler = new MockLogin();
            
            _loginPage = GetComponentInChildren<LoginPage>();
            _loginPage.SetupLogin(loginHandler);
            _loginPage.LoginHandler.OnMFAEmailSent += OnMFAEmailSentHandler;
            _loginPage.LoginHandler.OnMFAEmailFailedToSend += OnMFAEmailFailedToSendHandler;

            _mfaPage = GetComponentInChildren<MultifactorAuthenticationPage>();
            _mfaPage.SetupLogin(loginHandler);
            _mfaPage.LoginHandler.OnLoginSuccess += OnLoginSuccessHandler;
            _mfaPage.LoginHandler.OnLoginFailed += OnLoginFailedHandler;

            _loginSuccessPage = GetComponentInChildren<LoginSuccessPage>();
        }

        public void OpenTransitionPanel()
        {
            _transitionPanel.OpenWithDelay(_closeAnimationDurationInSeconds);
        }

        private void OnLoginSuccessHandler(string userId)
        {
            Debug.Log($"Successful login as user ID: {userId}");
            StartCoroutine(SetUIPage(_loginSuccessPage));
        }

        private void OnLoginFailedHandler(string error)
        {
            Debug.Log($"Failed login: {error}");
        }

        private void OnMFAEmailSentHandler(string email)
        {
            Debug.Log($"Successfully sent MFA email to {email}");
            StartCoroutine(SetUIPage(_mfaPage, email));
        }

        private void OnMFAEmailFailedToSendHandler(string email, string error)
        {
            Debug.Log($"Failed to send MFA email to {email} with error: {error}");
        }
    }
}