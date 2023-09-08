using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Sequence.Authentication;
using Sequence.Demo.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Sequence.Demo
{
    public class SequenceUI : MonoBehaviour
    {
        public static bool IsTesting = false;
        
        private LoginPanel _loginPanel;
        private ConnectPage _connectPage;
        private LoginPage _loginPage;
        private MultifactorAuthenticationPage _mfaPage;
        private LoginSuccessPage _loginSuccessPage;

        private WalletPanel _walletPanel;
        private WalletPage _walletPage;
        private TokenInfoPage _tokenInfoPage;

        private UIPage _page;
        private Stack<PageWithArgs> _pageStack = new Stack<PageWithArgs>();
        
        private struct PageWithArgs
        {
            public UIPage page;
            public object[] args;

            public PageWithArgs(UIPage page, object[] args)
            {
                this.page = page;
                this.args = args;
            }
        }

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

            _walletPanel = GetComponentInChildren<WalletPanel>();

            _walletPage = GetComponentInChildren<WalletPage>();
            _tokenInfoPage = GetComponentInChildren<TokenInfoPage>();
        }

        public void Start()
        {
            if (IsTesting)
            {
                return;
            }
            DisableAllUIPages();
            OpenUIPanel(_loginPanel);
        }

        private void DisableAllUIPages()
        {
            UIPage[] pages = GetComponentsInChildren<UIPage>();
            int count = pages.Length;
            for (int i = 0; i < count; i++)
            {
                pages[i].gameObject.SetActive(false);
            }
        }

        public void OpenUIPanel(UIPanel panel, params object[] openArgs)
        {
            panel.Open(openArgs);
        }

        public IEnumerator SetUIPage(UIPage page, params object[] openArgs)
        {
            if (_page != null)
            {
                _page.Close();
                yield return new WaitUntil(() => !_page.isActiveAndEnabled);
            }
            _page = page;
            _pageStack.Push(new PageWithArgs(page, openArgs));
            _page.Open(openArgs);
        }

        public void Back()
        {
            if (_pageStack.Count <= 1)
            {
                return;
            }

            _pageStack.Pop().page.Close();
            PageWithArgs previous = _pageStack.Peek();
            _page = previous.page;
            _page.Open(previous.args);
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

        public void SwitchToTokenInfoPage(TokenElement tokenElement, NetworkIcons networkIcons, ITransactionDetailsFetcher transactionDetailsFetcher)
        {
            StartCoroutine(SetUIPage(_tokenInfoPage, tokenElement, networkIcons, transactionDetailsFetcher));
        }

        public void ClearStack()
        {
            _pageStack = new Stack<PageWithArgs>();
            _page = null;
        }
    }
}