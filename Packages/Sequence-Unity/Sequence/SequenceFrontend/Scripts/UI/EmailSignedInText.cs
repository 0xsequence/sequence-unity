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
        private WaaS.SequenceWallet _wallet;

        void Awake()
        {
            WaaS.SequenceWallet.OnWalletCreated += wallet =>
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
