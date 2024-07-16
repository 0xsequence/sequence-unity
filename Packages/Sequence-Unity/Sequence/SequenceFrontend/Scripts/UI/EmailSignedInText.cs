using System.Collections;
using System.Collections.Generic;
using Sequence.Authentication;
using Sequence.WaaS;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class EmailSignedInText : MonoBehaviour
    {
        private EmbeddedWallet _wallet;

        void Awake()
        {
            EmbeddedWallet.OnWalletCreated += wallet =>
            {
                TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
                text.text = "Logged in as: " + PlayerPrefs.GetString(OpenIdAuthenticator.LoginEmail);
                _wallet = wallet;
            };
        }

        public void SignOut()
        {
            _wallet.DropThisSession();
        }
    }
}
