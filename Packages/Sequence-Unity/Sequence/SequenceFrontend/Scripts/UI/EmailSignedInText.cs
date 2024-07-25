using System.Collections;
using System.Collections.Generic;
using Sequence.Authentication;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class EmailSignedInText : MonoBehaviour
    {
        private SequenceWallet _wallet;

        void Awake()
        {
            SequenceWallet.OnWalletCreated += wallet =>
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
