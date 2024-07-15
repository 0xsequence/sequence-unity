using System;
using System.Collections;
using System.Collections.Generic;
using Sequence.Authentication;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.Demo
{
    public class FederatedAuthPopupPanel : UIPanel
    {
        [FormerlySerializedAs("_overrideAccountConfirmationPage")] [SerializeField] private  NewAccountConfirmationPage newAccountConfirmationPage;
        private LoginPanel _loginPanel;
        private ILogin _login;
        private string _email;
        private LoginMethod _loginMethod;

        protected override void Awake()
        {
            base.Awake();
            _loginPanel = FindObjectOfType<LoginPanel>();
        }

        private void OnDestroy()
        {
            _login.OnLoginFailed -= OnLoginFailed;
        }

        public override IEnumerator OpenInitialPage(params object[] openArgs)
        {
            List<LoginMethod> loginMethods = _login.GetLoginMethodsAssociatedWithEmail(_email);
            yield return base.OpenInitialPage(openArgs, loginMethods, _loginMethod);
        }
        
        public void ReturnToLogin()
        {
            Close();
            _loginPanel.OpenWithDelay(_closeAnimationDurationInSeconds);
        }
        
        public void OverrideAccount()
        {
            StartCoroutine(SetUIPage(newAccountConfirmationPage, this, _login));
        }

        public void OnLoginFailed(string message, LoginMethod method, string email)
        {
            if (!string.IsNullOrWhiteSpace(email) && message.Contains("EmailAlreadyInUse"))
            {
                _email = email;
                _loginMethod = method;
                Open();
            }
        }
        
        public void InjectILogin(ILogin login)
        {
            _login = login;
            _login.OnLoginFailed += OnLoginFailed;
        }
    }
}