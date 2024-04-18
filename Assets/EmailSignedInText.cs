using System.Collections;
using System.Collections.Generic;
using Sequence.Authentication;
using Sequence.WaaS;
using TMPro;
using UnityEngine;

public class EmailSignedInText : MonoBehaviour
{
    private WaaSWallet _wallet;
    
    void Awake()
    {
        WaaSWallet.OnWaaSWalletCreated += wallet =>
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
