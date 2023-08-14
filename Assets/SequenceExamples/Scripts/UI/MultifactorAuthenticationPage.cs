using System;
using Sequence.Authentication;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class MultifactorAuthenticationPage : UIPage
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI[] _inputBoxes;
        [SerializeField] private TextMeshProUGUI _enterCodeText;

        private int _numberOfMFADigits;
        internal ILogin LoginHandler { get; private set; }

        private string email;

        protected override void Awake()
        {
            base.Awake();
            _inputField.onValueChanged.AddListener(OnInputValueChanged);
            _numberOfMFADigits = _inputBoxes.Length;
        }

        public void SetupLogin(ILogin loginHandler)
        {
            LoginHandler = loginHandler;
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            if (email == null && (args.Length != 1 || args[0] is not string))
            {
                throw new ArgumentException($"Expected exactly one argument of type {typeof(string)}");
            }

            if (args.Length > 0)
            {
                email = (string)args[0];
            }
            _enterCodeText.text = $"Enter the code sent to\n<b>{email}</b>";
            _inputField.text = "";
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
            string code = _inputField.text.Substring(0, _numberOfMFADigits);
            Debug.Log($"Attempting to sign in with email {email} and code {code}");
            LoginHandler.Login(email, code);
        }
    }
}