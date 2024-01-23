using System;
using System.Collections;
using System.Collections.Generic;
using Sequence.Demo;
using Sequence.WaaS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSceneUIManager : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    
    private LoginPanel _loginPanel;
    private void Awake()
    {
        _loginPanel = GetComponentInChildren<LoginPanel>();
    }

    private void Start()
    {
        CloseAllUIPages();
        _loginPanel.Open();
        WaaSWallet.OnWaaSWalletCreated += OnWaaSWalletCreatedHandler;
    }

    private void CloseAllUIPages()
    {
        UIPage[] pages = GetComponentsInChildren<UIPage>();
        int count = pages.Length;
        for (int i = 0; i < count; i++)
        {
            pages[i].gameObject.SetActive(false);
        }
    }
    
    private void OnWaaSWalletCreatedHandler(WaaSWallet wallet)
    {
        SequenceBridge.Wallet = wallet;
        wallet.OnSendTransactionComplete += SequenceBridge.OnTransactionSuccessHandler;
        wallet.OnSendTransactionFailed += SequenceBridge.OnTransactionFailedHandler;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
