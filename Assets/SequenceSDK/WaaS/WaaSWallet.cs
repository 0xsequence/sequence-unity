using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Authentication;
using Sequence.WaaS.Authentication;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.WaaS
{
    public class WaaSWallet : IWallet
    {
        public static Action<WaaSWallet> OnWaaSWalletCreated; 
        public string SessionId { get; private set; }
        private Address _address;
        private HttpClient _httpClient;
        private IntentSender _intentSender;

        public WaaSWallet(Address address, string sessionId, EthWallet sessionWallet, DataKey awsDataKey, int waasProjectId, string waasVersion)
        {
            _address = address;
            _httpClient = new HttpClient("https://d14tu8valot5m0.cloudfront.net/rpc/WaasWallet");
            _intentSender = new IntentSender(new HttpClient(WaaSLogin.WaaSWithAuthUrl), awsDataKey, sessionWallet, sessionId, waasProjectId, waasVersion);
            SessionId = sessionId;
        }

        public Address GetWalletAddress()
        {
            return _address;
        }

        public event Action<SignMessageReturn> OnSignMessageComplete;

        public async Task<SignMessageReturn> SignMessage(SignMessageArgs args)
        {
            var result = await _intentSender.SendIntent<SignMessageReturn, SignMessageArgs>(args);
            OnSignMessageComplete?.Invoke(result);
            return result;
        }

        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(IsValidMessageSignatureArgs args) // Todo figure out this intent is still supported
        {
            return _intentSender.SendIntent<IsValidMessageSignatureReturn, IsValidMessageSignatureArgs>(args);
        }

        public event Action<SuccessfulTransactionReturn> OnSendTransactionComplete;
        public event Action<FailedTransactionReturn> OnSendTransactionFailed;

        public async Task<TransactionReturn> SendTransaction(SendTransactionArgs args)
        {
            var result = await _intentSender.SendIntent<TransactionReturn, SendTransactionArgs>(args);
            if (result is SuccessfulTransactionReturn)
            {
                OnSendTransactionComplete?.Invoke((SuccessfulTransactionReturn)result);
            }
            else
            {
                OnSendTransactionFailed?.Invoke((FailedTransactionReturn)result);
            }
            return result;
        }

        public event Action<string> OnDropSessionComplete; 

        public async Task<bool> DropSession(string dropSessionId)
        {
            var result = await _intentSender.DropSession(dropSessionId);
            if (result)
            {
                OnDropSessionComplete?.Invoke(dropSessionId);
            }
            else
            {
                Debug.LogError("Failed to drop session: " + dropSessionId);
            }
            return result;
        }

        public Task<bool> DropThisSession()
        {
            return DropSession(SessionId);
        }

        public event Action<WaaSSession[]> OnSessionsFound;
        public async Task<WaaSSession[]> ListSessions()
        {
            WaaSSession[] results = await _intentSender.ListSessions();
            OnSessionsFound?.Invoke(results);
            return results;
        }
    }
}