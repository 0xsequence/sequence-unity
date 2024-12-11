using System;
using System.Collections;
using System.Threading.Tasks;
using Sequence.Authentication;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;

namespace GuestLogin
{
    public class GuestLogin : MonoBehaviour
    {
        [SerializeField] private bool _loginAutomatically = true;
        
        private ILogin _login;
        private int _signOuts = 0;
        private IWallet _wallet;
        private void Awake()
        {
            _login = SequenceLogin.GetInstance();
            _login.OnLoginFailed += (error, method, email, methods) =>
            {
                Debug.LogError(error);
            };
            SequenceWallet.OnWalletCreated += OnWaaSWalletCreated;
        }

        private void Start()
        {
            if (_loginAutomatically)
            {
                _login.GuestLogin();
            }
        }

        private void OnWaaSWalletCreated(SequenceWallet wallet)
        {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            text.text = "Logged in as: " + wallet.GetWalletAddress();
            _wallet = wallet;
            
            StartCoroutine(SignOutThenSignBackIn());
        }

        private IEnumerator SignOutThenSignBackIn()
        {
            if (_signOuts > 1)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(3f);
                
                TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
                text.text = "Logged out";

                Task signOutTask = _wallet.DropThisSession();
                yield return new WaitUntil(() => signOutTask.IsCompleted);
                _signOuts++;
                _wallet = null;
                
                _login.GuestLogin();
            }
        }

        public void Login()
        {
            _login.GuestLogin();
        }
    }
}