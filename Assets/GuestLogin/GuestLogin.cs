using System;
using System.Collections;
using System.Threading.Tasks;
using Sequence.Authentication;
using Sequence.WaaS;
using TMPro;
using UnityEngine;

namespace GuestLogin
{
    public class GuestLogin : MonoBehaviour
    {
        private ILogin _login;
        private int _signOuts = 0;
        private IWallet _wallet;
        private void Awake()
        {
            _login = WaaSLogin.GetInstance();
            _login.OnLoginFailed += (error, method, email) =>
            {
                Debug.LogError(error);
            };
            EmbeddedWallet.OnWalletCreated += OnWaaSWalletCreated;
        }

        private void Start()
        {
            _login.GuestLogin();
        }

        private void OnWaaSWalletCreated(EmbeddedWallet wallet)
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
    }
}