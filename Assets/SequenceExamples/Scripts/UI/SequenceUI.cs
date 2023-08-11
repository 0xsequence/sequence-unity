using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Sequence.Demo
{
    public class SequenceUI : UIPage
    {
        private ConnectPage _connectPage;
        private LoginPage _loginPage;
        private LoginSuccessPage _loginSuccessPage;

        private UIPage _page;
        private Stack<UIPage> _pageStack = new Stack<UIPage>();

        protected override void Awake()
        {
            base.Awake();
            
            _connectPage = GetComponentInChildren<ConnectPage>();
            
            _loginPage = GetComponentInChildren<LoginPage>();
            _loginPage.SetupLogin(new MockLogin());
            _loginPage.LoginHandler.OnLoginSuccess += OnLoginSuccessHandler;
            _loginPage.LoginHandler.OnLoginFailed += OnLoginFailedHandler;

            _loginSuccessPage = GetComponentInChildren<LoginSuccessPage>();
        }

        protected override void Start()
        {
            base.Start();
            Open();
            StartCoroutine(SetInitialUIPage(_loginPage));
        }

        private IEnumerator SetInitialUIPage(UIPage page)
        {
            yield return new WaitForSeconds(base._openAnimationDurationInSeconds);
            yield return new WaitUntil(() => page.SetupComplete);
            _page = page;
            _pageStack.Push(page);
            _page.Open();
        }

        public IEnumerator SetUIPage(UIPage page)
        {
            _page.Close();
            yield return new WaitUntil(() => !_page.isActiveAndEnabled);
            _page = page;
            _pageStack.Push(page);
            _page.Open();
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
    }
}