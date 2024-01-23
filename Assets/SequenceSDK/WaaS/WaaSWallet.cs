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
        private IIntentSender _intentSender;
        private const string _sequenceCreatedContractEvent = "CreatedContract(address)";

        public WaaSWallet(Address address, string sessionId, IIntentSender intentSender)
        {
            _address = address;
            _httpClient = new HttpClient("https://api.sequence.app/rpc");
            _intentSender = intentSender;
            SessionId = sessionId;
        }

        public Address GetWalletAddress()
        {
            return _address;
        }

        public event Action<SignMessageReturn> OnSignMessageComplete;

        public async Task<SignMessageReturn> SignMessage(Chain network, string message, uint timeBeforeExpiry = 30)
        {
            SignMessageArgs args = new SignMessageArgs(_address, network, message, timeBeforeExpiry);
            var result = await _intentSender.SendIntent<SignMessageReturn, SignMessageArgs>(args);
            OnSignMessageComplete?.Invoke(result);
            return result;
        }

        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(Chain network, string message, string signature)
        {
            return _httpClient.SendRequest<IsValidMessageSignatureArgs, IsValidMessageSignatureReturn>(
                "API/IsValidMessageSignature", new IsValidMessageSignatureArgs(network, _address, message, signature));
        }

        public event Action<SuccessfulTransactionReturn> OnSendTransactionComplete;
        public event Action<FailedTransactionReturn> OnSendTransactionFailed;

        public async Task<TransactionReturn> SendTransaction(Chain network, Transaction[] transactions, uint timeBeforeExpiry = 30)
        {
            SendTransactionArgs args = new SendTransactionArgs(_address, network, transactions, timeBeforeExpiry);
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

        public event Action<SuccessfulContractDeploymentReturn> OnDeployContractComplete;
        public event Action<FailedContractDeploymentReturn> OnDeployContractFailed;

        public async Task<ContractDeploymentReturn> DeployContract(Chain network, string bytecode, string value = "0")
        {
            bytecode = bytecode.EnsureHexPrefix();
            TransactionReturn transactionReturn = await SendTransaction(
                network,
                new Transaction[]
                {
                    new DelayedEncode(_address, value, new DelayedEncodeData(
                        "createContract(bytes)", new object[]
                        {
                            bytecode,
                        }, "createContract"))
                });
            
            if (transactionReturn is SuccessfulTransactionReturn successfulTransactionReturn)
            {
                string topic = SequenceCoder.KeccakHashASCII(_sequenceCreatedContractEvent);
                MetaTxnReceiptLog log = FindLogWithTopic(successfulTransactionReturn.receipt, topic);
                if (log == null)
                {
                    FailedContractDeploymentReturn failedResult = new FailedContractDeploymentReturn(null,
                        "Failed to find newly deployed contract address in transaction receipt logs " +
                        successfulTransactionReturn.receipt);
                    OnDeployContractFailed?.Invoke(failedResult);
                    return failedResult;
                }
                string deployedContractAddressString = log.data.RemoveZeroPadding();
                Address deployedContractAddress = new Address(deployedContractAddressString);
                
                SuccessfulContractDeploymentReturn result = new SuccessfulContractDeploymentReturn(successfulTransactionReturn, deployedContractAddress);
                OnDeployContractComplete?.Invoke(result);
                return result;
            }
            else if (transactionReturn is FailedTransactionReturn failedTransactionReturn)
            {
                FailedContractDeploymentReturn result = new FailedContractDeploymentReturn(failedTransactionReturn, failedTransactionReturn.error);
                OnDeployContractFailed?.Invoke(result);
                return result;
            }
            else
            {
                FailedContractDeploymentReturn result = new FailedContractDeploymentReturn(null,
                    $"Unknown transaction result type. Given {transactionReturn.GetType().Name}");
                OnDeployContractFailed?.Invoke(result);
                return result;
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