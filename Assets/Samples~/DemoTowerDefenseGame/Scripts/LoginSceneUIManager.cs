using System;
using System.Collections;
using System.Collections.Generic;
using Sequence.Demo;
using Sequence.Relayer;
using Sequence.WaaS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSceneUIManager : MonoBehaviour
{ 
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
        
        SequenceBridge.Minter = new CloudflareMinter(new MintingRequestProver(wallet, SequenceBridge.Network), "https://sequence-relayer-jelly-forest.tpin.workers.dev/", SequenceBridge.GameStateContractAddress);
        SequenceBridge.Minter.OnMintTokenSuccess += SequenceBridge.OnMintSuccessHandler;
        SequenceBridge.Minter.OnMintTokenFailed += SequenceBridge.OnMintFailedHandler;
        
        SceneManager.LoadScene("MainMenu");
    }
    
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
