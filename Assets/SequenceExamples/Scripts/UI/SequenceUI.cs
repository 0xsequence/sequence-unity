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
        }

        protected override void Start()
        {
            base.Start();
            Open();
            StartCoroutine(SetInitialUIPage(_loginPage));
        }

        private IEnumerator SetInitialUIPage(UIPage page)
        {
            yield return new WaitForSecondsRealtime(base._openAnimationDurationInSeconds);
            yield return new WaitUntil(() => page.SetupComplete);
            _page = page;
            _pageStack.Push(page);
            _page.Open();
        }

        public void SetUIPage(UIPage page)
        {
            _page.Close();
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
            Debug.Log("successful login");
        }

        private void OnLoginFailedHandler(string error)
        {
            Debug.Log("failed login");
        }
    }
}