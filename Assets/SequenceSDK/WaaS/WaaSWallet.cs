using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Authentication;
using Sequence.Utils;
using Sequence.WaaS.Authentication;
using Sequence.Wallet;
using SequenceSDK.WaaS;
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
        private const string _sequenceCreatedContractEvent = "CreatedContract(address)";

        public WaaSWallet(Address address, string sessionId, EthWallet sessionWallet, DataKey awsDataKey, int waasProjectId, string waasVersion)
        {
            _address = address;
            _httpClient = new HttpClient("https://api.sequence.app/rpc");
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

        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(IsValidMessageSignatureArgs args)
        {
            return _httpClient.SendRequest<IsValidMessageSignatureArgs, IsValidMessageSignatureReturn>(
                "API/IsValidMessageSignature", args);
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

        public event Action<SuccessfulContractDeploymentResult> OnDeployContractComplete;
        public event Action<FailedContractDeploymentResult> OnDeployContractFailed;

        public async Task<SuccessfulContractDeploymentResult> DeployContract(Chain network, string bytecode, string value = "0")
        {
            bytecode = bytecode.EnsureHexPrefix();
            TransactionReturn transactionReturn = await SendTransaction(new SendTransactionArgs(
                _address,
                network,
                new Transaction[]
                {
                    new DelayedEncode(_address, value, new DelayedEncodeData(
                        "createContract(bytes)", new object[]
                        {
                            bytecode,
                        }, "createContract"))
                }));
            
            if (transactionReturn is SuccessfulTransactionReturn successfulTransactionReturn)
            {
                string topic = SequenceCoder.KeccakHashASCII(_sequenceCreatedContractEvent);
                MetaTxnReceiptLog log = FindLogWithTopic(successfulTransactionReturn.receipt, topic);
                if (log == null)
                {
                    OnDeployContractFailed?.Invoke(new FailedContractDeploymentResult(null,"Failed to find newly deployed contract address in transaction receipt logs " + successfulTransactionReturn.receipt));
                }
                string deployedContractAddressString = log.data.RemoveZeroPadding();
                Address deployedContractAddress = new Address(deployedContractAddressString);
                
                SuccessfulContractDeploymentResult result = new SuccessfulContractDeploymentResult(successfulTransactionReturn, deployedContractAddress);
                OnDeployContractComplete?.Invoke(result);
                return result;
            }
            else if (transactionReturn is FailedTransactionReturn failedTransactionReturn)
            {
                OnDeployContractFailed?.Invoke(new FailedContractDeploymentResult(failedTransactionReturn, failedTransactionReturn.error));
                return null;
            }
            else
            {
                OnDeployContractFailed?.Invoke(new FailedContractDeploymentResult(null, $"Unknown transaction result type. Given {transactionReturn.GetType().Name}"));
                return null;
            }
        }
        
        private MetaTxnReceiptLog FindLogWithTopic(MetaTxnReceipt receipt, string topic)
        {
            topic = topic.EnsureHexPrefix();
            MetaTxnReceipt[] receipts = receipt.receipts;
            int receiptsLength = receipts.Length;
            for (int i = 0; i < receiptsLength; i++)
            {
                int logsCount = receipts[i].logs.Length;
                for (int j = 0; j < logsCount; j++)
                {
                    int topicsCount = receipts[i].logs[j].topics.Length;
                    for (int k = 0; k < topicsCount; k++)
                    {
                        if (receipts[i].logs[j].topics[k] == topic)
                        {
                            return receipts[i].logs[j];
                        }
                    }
                }
            }

            return null;
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