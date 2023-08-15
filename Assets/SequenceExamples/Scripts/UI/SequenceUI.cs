using System;
using System.Collections;
using System.Collections.Generic;
using Sequence.Authentication;
using Unity.VisualScripting;
using UnityEngine;

namespace Sequence.Demo
{
    public class SequenceUI : MonoBehaviour
    {
        private LoginPanel _loginPanel;
        private ConnectPage _connectPage;
        private LoginPage _loginPage;
        private MultifactorAuthenticationPage _mfaPage;
        private LoginSuccessPage _loginSuccessPage;

        private UIPage _page;
        private Stack<UIPage> _pageStack = new Stack<UIPage>();

        private void Awake()
        {
            _loginPanel = GetComponentInChildren<LoginPanel>();
            
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

        private void Start()
        {
            StartCoroutine(SetInitialUIPage(_loginPanel, _loginPage));
        }

        private IEnumerator SetInitialUIPage(UIPanel panel, UIPage page)
        {
            yield return new WaitUntil(() => panel.SetupComplete);
            panel.Open();
            yield return new WaitUntil(() => page.SetupComplete);
            _page = page;
            _pageStack.Push(page);
            StartCoroutine(panel.OpenInitialPage(page));
        }

        public IEnumerator SetUIPage(UIPage page, params object[] openArgs)
        {
            _page.Close();
            yield return new WaitUntil(() => !_page.isActiveAndEnabled);
            _page = page;
            _pageStack.Push(page);
            _page.Open(openArgs);
        }

        public void Back()
        {
            if (_pageStack.Count <= 1)
            {
                return;
            }

            _pageStack.Pop().Close();
            _page = _pageStack.Peek();
            _page.Open();
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