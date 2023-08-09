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
        
        public enum UIState
        {
            Connect,
            Login,
            TwoFactorAuthentication,
            Wallet,
            Default,
        }

        public UIState State { get; private set; } = UIState.Default;

        private Stack<UIState> _stateStack = new Stack<UIState>();

        private void Awake()
        {
            _connectPage = FindObjectOfType<ConnectPage>();
            _loginPage = FindObjectOfType<LoginPage>();
        }

        private void Start()
        {
            SetUIState(UIState.Connect);
        }

        public void SetUIState(UIState desiredState)
        {
            ClosePage();
            State = desiredState;
            _stateStack.Push(desiredState);
            OpenPage();
        }

        private void ClosePage()
        {
            switch (State)
            {
                case UIState.Connect:
                    _connectPage.Close();
                    break;
                case UIState.Login:
                    _loginPage.Close();
                    break;
                case UIState.TwoFactorAuthentication:
                    break;
                case UIState.Wallet:
                    break;
            }
        }

        private void OpenPage()
        {
            switch (State)
            {
                case UIState.Connect:
                    _connectPage.Open();
                    break;
                case UIState.Login:
                    _loginPage.Open();
                    break;
                case UIState.TwoFactorAuthentication:
                    break;
                case UIState.Wallet:
                    break;
            }
        }

        public void Back()
        {
            if (_stateStack.Count <= 1)
            {
                return;
            }

            ClosePage();
            _stateStack.Pop();
            State = _stateStack.Peek();
            OpenPage();
        }

        public void OpenLoginPage()
        {
            SetUIState(UIState.Login);
        }
    }
}