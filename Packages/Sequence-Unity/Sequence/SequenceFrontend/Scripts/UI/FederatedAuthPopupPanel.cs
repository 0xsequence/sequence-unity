using System;
using System.Collections;
using System.Collections.Generic;
using Sequence.Authentication;
using Sequence.EmbeddedWallet;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.Demo
{
    public class FederatedAuthPopupPanel : UIPanel
    {
        [FormerlySerializedAs("_overrideAccountConfirmationPage")] 
        [SerializeField] private  NewAccountConfirmationPage newAccountConfirmationPage;
        
        private LoginPanel _loginPanel;
        private ILogin _login;
        private string _email;
        private LoginMethod _loginMethod;
        private List<LoginMethod> _loginMethods;

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
            yield return base.OpenInitialPage(openArgs, _loginMethods, _loginMethod);
        }
        
        public void ReturnToLogin()
        {
            Close();
            _loginPanel.Open();
        }
        
        public void OverrideAccount()
        {
            StartCoroutine(SetUIPage(newAccountConfirmationPage, this, _login));
        }

        public void OnLoginFailed(string message, LoginMethod method, string email, List<LoginMethod> loginMethods = default)
        {
            if (!string.IsNullOrWhiteSpace(email) && message.Contains(SequenceLogin.EmailInUseError))
            {
                _email = email;
                _loginMethod = method;
                _loginMethods = loginMethods;
                if (_loginMethods == default)
                {
                    throw new Exception("No login methods provided");
                }
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