using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Demo
{
    public class SequenceUI : MonoBehaviour
    {
        private ConnectPage _connectPage;
        private LoginPage _loginPage;

        private UIPage _page;
        private Stack<UIPage> _pageStack = new Stack<UIPage>();

        private void Awake()
        {
            _connectPage = FindObjectOfType<ConnectPage>();
            _loginPage = FindObjectOfType<LoginPage>();
        }

        private void Start()
        {
            SetInitialUIPage(_connectPage);
        }

        private void SetInitialUIPage(UIPage page)
        {
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
    }
}