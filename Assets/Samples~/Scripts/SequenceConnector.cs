using System;
using Sequence;
using Sequence.WaaS;
using Sequence.WaaS.Authentication;
using UnityEngine;
using UnityEngine.Serialization;

namespace Samples.Scripts
{
    /// <summary>
    /// Attach this to a GameObject in your scene. It will automatically capture a WaaSWallet when it is created and setup all event handlers (fill in your own logic).
    /// This mono behaviour will persist between scenes and is accessed via SequenceConnector.Instance singleton.
    /// </summary>
    public class SequenceConnector : MonoBehaviour
    {
        public Chain Chain { get; private set; }
        public static SequenceConnector Instance { get; private set; }
        
        public WaaSWallet Wallet { get; private set; }
        public IIndexer Indexer { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }

            WaaSWallet.OnWaaSWalletCreated += OnWaaSWalletCreated;
            Indexer = new ChainIndexer(Chain);
        }

        private void OnWaaSWalletCreated(WaaSWallet wallet)
        {
            Wallet = wallet;
            Wallet.OnSendTransactionComplete += OnSendTransactionCompleteHandler;
            Wallet.OnSendTransactionFailed += OnSendTransactionFailedHandler;
            Wallet.OnSignMessageComplete += OnSignMessageCompleteHandler;
            Wallet.OnDeployContractComplete += OnDeployContractCompleteHandler;
            Wallet.OnDeployContractFailed += OnDeployContractFailedHandler;
            Wallet.OnDropSessionComplete += OnDropSessionCompleteHandler;
            Wallet.OnSessionsFound += OnSessionsFoundHandler;
        }

        private void OnDestroy()
        {
            Wallet.OnSendTransactionComplete -= OnSendTransactionCompleteHandler;
            Wallet.OnSendTransactionFailed -= OnSendTransactionFailedHandler;
            Wallet.OnSignMessageComplete -= OnSignMessageCompleteHandler;
            Wallet.OnDeployContractComplete -= OnDeployContractCompleteHandler;
            Wallet.OnDeployContractFailed -= OnDeployContractFailedHandler;
            Wallet.OnDropSessionComplete -= OnDropSessionCompleteHandler;
            Wallet.OnSessionsFound -= OnSessionsFoundHandler;
        }

        private void OnSendTransactionCompleteHandler(SuccessfulTransactionReturn result) {
            // Do something
        }

        private void OnSendTransactionFailedHandler(FailedTransactionReturn result) {
            // Do something
        }
        
        private void OnSignMessageCompleteHandler(string result) {
            // Do something
        }
        
        private void OnDeployContractCompleteHandler(SuccessfulContractDeploymentReturn result) {
            Address newlyDeployedContractAddress = result.DeployedContractAddress;

            // Do something
        }

        private void OnDeployContractFailedHandler(FailedContractDeploymentReturn result) {
            // Do something
        }
        
        private void OnDropSessionCompleteHandler(string sessionId) {
            // Do something
        }
        
        private void OnSessionsFoundHandler(string[] sessions) {
            // Do something
        }
    }
}