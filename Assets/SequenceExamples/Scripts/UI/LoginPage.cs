using Sequence.Authentication;
using Sequence.Demo.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class LoginPage : UIPage
    {
        private TMP_InputField _inputField;
        internal ILogin LoginHandler { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _inputField = GetComponentInChildren<TMP_InputField>();
        }

        public void SetupLogin(ILogin loginHandler)
        {
            LoginHandler = loginHandler;
        }

        public void Login()
        {
            string email = _inputField.text;
            Debug.Log($"Signing in with email: {email}");
            LoginHandler.Login(email);
        }

        public void GoogleLogin()
        {
            Debug.Log("Google Login");
            LoginHandler.GoogleLogin();
        }

        public void DiscordLogin()
        {
            Debug.Log("Discord Login");
            LoginHandler.DiscordLogin();
        }

        public void FacebookLogin()
        {
            Debug.Log("Facebook Login");
            LoginHandler.FacebookLogin();
        }

        public void AppleLogin()
        {
            Debug.Log("Apple Login");
            LoginHandler.AppleLogin();
        }
    }
}