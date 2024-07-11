using System;
using Sequence.Authentication;
using Sequence.WaaS;
using TMPro;
using UnityEngine;

namespace GuestLogin
{
    public class GuestLogin : MonoBehaviour
    {
        private ILogin _login;
        private void Awake()
        {
            _login = WaaSLogin.GetInstance();
            _login.OnLoginFailed += error =>
            {
                Debug.LogError(error);
            };
            WaaSWallet.OnWaaSWalletCreated += wallet =>
            {
                TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
                text.text = "Logged in successful. Wallet Address: " + wallet.GetWalletAddress();
            };
        }

        private void Start()
        {
            _login.GuestLogin();
        }
    }
}