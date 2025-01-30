using System;
using System.Collections.Generic;
using Sequence.Authentication;
using Sequence.Utils;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class MultifactorAuthenticationPage : UIPage
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI[] _inputBoxes;
        [SerializeField] private TextMeshProUGUI _enterCodeText;
        [SerializeField] private TextMeshProUGUI _errorText;
        [SerializeField] private GameObject _loadingScreen;

        private int _numberOfMFADigits;
        internal ILogin LoginHandler { get; private set; }

        private string email;

        protected override void Awake()
        {
            base.Awake();
            _inputField.onValueChanged.AddListener(OnInputValueChanged);
            _numberOfMFADigits = _inputBoxes.Length;
        }

        private void Update()
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand))
                && Input.GetKeyDown(KeyCode.V))
            {
                _inputField.text = GUIUtility.systemCopyBuffer;
                Login();
            }
        }

        public override void Close()
        {
            base.Close();
            _errorText.text = "";
            
            LoginHandler.OnLoginSuccess -= OnLoginSuccessHandler;
            LoginHandler.OnLoginFailed -= OnLoginFailedHandler;
        }

        public void SetupLogin(ILogin loginHandler)
        {
            LoginHandler = loginHandler;
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            if (email == null)
            {
                email = args.GetObjectOfTypeIfExists<string>();
                
                if (email == default)
                {
                    throw new SystemException(
                        $"Invalid use. {GetType().Name} must be opened with a {typeof(string)} as an argument");
                }
            }

            if (args.Length > 0)
            {
                email = (string)args[0];
            }
            _enterCodeText.text = $"Enter the code sent to\n<b>{email}</b>";
            _inputField.text = "";
            
            LoginHandler.OnLoginSuccess += OnLoginSuccessHandler;
            LoginHandler.OnLoginFailed += OnLoginFailedHandler;
        }

        private void OnInputValueChanged(string newValue)
        {
            string validInput = newValue;
            if (newValue.Length > _numberOfMFADigits)
            {
                validInput = newValue.Substring(0, _numberOfMFADigits);
            }

            int length = validInput.Length;
            for (int i = 0; i < +_numberOfMFADigits; i++)
            {
                if (i < length)
                {
                    _inputBoxes[i].text = validInput[i].ToString();
                }
                else
                {
                    _inputBoxes[i].text = "";
                }
            }
        }

        public void Login()
        {
            string code = _inputField.text.Substring(0, Math.Min(_numberOfMFADigits, _inputField.text.Length));
            Debug.Log($"Attempting to sign in with email {email} and code {code}");
            LoginHandler.Login(email, code);
            EnableLoadingScreen(true);
        }

        private void OnLoginSuccessHandler(string sessionId, string walletAddress)
        {
            Debug.Log($"Login Succeeded {walletAddress}");
            EnableLoadingScreen(false);
        }
        
        private void OnLoginFailedHandler(string error, LoginMethod method, string email, List<LoginMethod> loginMethods)
        {
            Debug.LogError($"Failed login: {error}");
            _errorText.text = error;
            EnableLoadingScreen(false);
        }

        private void EnableLoadingScreen(bool enable)
        {
            _loadingScreen.SetActive(enable);
        }
    }
}