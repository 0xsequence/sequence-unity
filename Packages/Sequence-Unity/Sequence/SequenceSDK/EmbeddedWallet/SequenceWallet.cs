using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Config;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EmbeddedWallet
{
    public class SequenceWallet : IWallet
    {
        public static Action<SequenceWallet> OnWalletCreated;
        public static Action<string> OnFailedToRecoverSession;
        public static Action<Account> OnAccountFederated;
        public static Action<string> OnAccountFederationFailed;
        
        public string SessionId { get; private set; }
        private Address _address;
        private HttpClient _httpClient;
        private IIntentSender _intentSender;
        private const string _sequenceCreatedContractEvent = "CreatedContract(address)";
        private string _builderApiKey;
        private string _email;

        public SequenceWallet(Address address, string sessionId, IIntentSender intentSender, string email = "")
        {
            _address = address;
#if SEQUENCE_DEV_WAAS || SEQUENCE_DEV           
            _httpClient = new HttpClient("https://dev-api.sequence.app/rpc");
#else
            _httpClient = new HttpClient("https://api.sequence.app/rpc");
#endif            
            _intentSender = intentSender;
            SessionId = sessionId;
            _builderApiKey = SequenceConfig.GetConfig(SequenceService.WaaS).BuilderAPIKey;
            _email = email;
            
            OnAccountFederated += account =>
            {
                if (String.Equals(account.wallet, address, StringComparison.CurrentCultureIgnoreCase))
                {
                    _email = account.email;

                    SequenceLogin.GetInstance().StoreWalletSecurely(_address, _email);
                }
            };
        }

        public Address GetWalletAddress()
        {
            return _address;
        }

        public event Action<string> OnSignMessageComplete;
        public event Action<string> OnSignMessageFailed;

        public async Task<string> SignMessage(Chain network, string message, uint timeBeforeExpiry = 30)
        {
            try
            {
                IntentDataSignMessage args = new IntentDataSignMessage(_address, network, message);
                var result = await _intentSender.SendIntent<IntentResponseSignedMessage, IntentDataSignMessage>(args, IntentType.SignMessage, timeBeforeExpiry);
                string signature = result.signature;

                if (signature == "")
                {
                    throw new Exception("Message could not be signed.");
                }
                else
                {
                    OnSignMessageComplete?.Invoke(signature);
                    return signature;
                }
            }
            catch (Exception e)
            {
                OnSignMessageFailed?.Invoke(e.Message);
                return e.Message;
            }
        }

        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(Chain network, string message, string signature)
        {
            if (string.IsNullOrWhiteSpace(_builderApiKey))
            {
                throw SequenceConfig.MissingConfigError("Builder API Key");
            }

            return _httpClient.SendRequest<IsValidMessageSignatureArgs, IsValidMessageSignatureReturn>(
                "API/IsValidMessageSignature", new IsValidMessageSignatureArgs(network, _address, message, signature),
            new Dictionary<string, string>() {{"X-Access-Key", _builderApiKey}}); 
        }

        public event Action<SuccessfulTransactionReturn> OnSendTransactionComplete;
        public event Action<FailedTransactionReturn> OnSendTransactionFailed;

        public async Task<TransactionReturn> SendTransaction(Chain network, Transaction[] transactions, bool waitForReceipt = true, uint timeBeforeExpiry = 30)
        {
            IntentDataSendTransaction args = new IntentDataSendTransaction(_address, network, transactions);
            try
            {
                TransactionsValidator.Validate(transactions);
                return await SendTransactionIntent(args, waitForReceipt, timeBeforeExpiry);
            }
            catch (Exception e)
            {
                FailedTransactionReturn result = new FailedTransactionReturn(e.Message, args);
                OnSendTransactionFailed?.Invoke(result);
                return result;
            }
        }

        private async Task<TransactionReturn> SendTransactionIntent(IntentDataSendTransaction args, bool waitForReceipt, uint timeBeforeExpiry)
        {
            var result = await _intentSender.SendIntent<TransactionReturn, IntentDataSendTransaction>(args, IntentType.SendTransaction, timeBeforeExpiry);
            if (result is SuccessfulTransactionReturn successfulTransactionReturn)
            {
                if (waitForReceipt)
                {
                    while (string.IsNullOrWhiteSpace(successfulTransactionReturn.txHash))
                    {
                        try
                        {
                            successfulTransactionReturn = await _intentSender.GetTransactionReceipt(successfulTransactionReturn);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Transaction was successful, but we're unable to obtain the transaction hash. Reason: " + e.Message);
                            OnSendTransactionComplete?.Invoke(successfulTransactionReturn);
                            return result;
                        }
                    }

                    OnSendTransactionComplete?.Invoke(successfulTransactionReturn);
                    return successfulTransactionReturn;
                }
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
                Debug.LogError("Failed to drop sessionId: " + dropSessionId);
            }
            return result;
        }

        public Task<bool> DropThisSession()
        {
            return DropSession(SessionId);
        }

        public event Action<Session[]> OnSessionsFound;
        public async Task<Session[]> ListSessions()
        {
            Session[] results = null;
            try
            {
                results = await _intentSender.ListSessions(GetWalletAddress());
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to list sessions: " + e.Message);
            }
            OnSessionsFound?.Invoke(results);
            return results;
        }

        public async Task<SuccessfulTransactionReturn> WaitForTransactionReceipt(SuccessfulTransactionReturn successfulTransactionReturn)
        {
            while (string.IsNullOrWhiteSpace(successfulTransactionReturn.txHash))
            {
                try
                {
                    successfulTransactionReturn = await _intentSender.GetTransactionReceipt(successfulTransactionReturn);
                }
                catch (Exception e)
                {
                    Debug.LogError("Transaction was successful, but we're unable to obtain the transaction hash. Reason: " + e.Message);
                    return successfulTransactionReturn;
                }
            }

            return successfulTransactionReturn;
        }

        public async Task<FeeOptionsResponse> GetFeeOptions(Chain network, Transaction[] transactions, uint timeBeforeExpiry = 30)
        {
            IntentDataFeeOptions feeOptions = new IntentDataFeeOptions(network, _address, transactions);
            try
            {
                TransactionsValidator.Validate(transactions);
                IntentResponseFeeOptions options = await
                    _intentSender.SendIntent<IntentResponseFeeOptions, IntentDataFeeOptions>(feeOptions, IntentType.FeeOptions,
                        timeBeforeExpiry);

                FeeOptionsResponse feeOptionsResponse = await DetermineWhichFeeOptionsUserHasInWallet(options, network);
                
                return feeOptionsResponse; 
            }
            catch (Exception e)
            {
                OnSendTransactionFailed?.Invoke(new FailedTransactionReturn($"Unable to get fee options: {e.Message}", 
                    new IntentDataSendTransaction(_address, network, transactions)));
                return null;
            }
        }

        private async Task<FeeOptionsResponse> DetermineWhichFeeOptionsUserHasInWallet(IntentResponseFeeOptions feeOptions, Chain network)
        {
            try
            {
                IIndexer indexer = new ChainIndexer(network);
                int feeOptionsLength = feeOptions.feeOptions.Length;
                FeeOptionReturn[] decoratedFeeOptions = new FeeOptionReturn[feeOptionsLength];
                for (int i = 0; i < feeOptionsLength; i++)
                {
                    FeeToken token = feeOptions.feeOptions[i].token;
                    BigInteger requiredBalance = BigInteger.Parse(feeOptions.feeOptions[i].value);
                    switch (token.type)
                    {
                        case FeeTokenType.unknown:
                            EtherBalance etherBalance = await indexer.GetEtherBalance(_address);
                            decoratedFeeOptions[i] = new FeeOptionReturn(feeOptions.feeOptions[i],
                                requiredBalance <= etherBalance.balanceWei);
                            break;
                        case FeeTokenType.erc20Token:
                            GetTokenBalancesReturn tokenBalances = await indexer.GetTokenBalances(new GetTokenBalancesArgs(
                                _address, token.contractAddress));
                            if (tokenBalances.balances.Length > 0)
                            {
                                if (tokenBalances.balances[0].contractAddress != token.contractAddress)
                                {
                                    throw new Exception(
                                        $"Expected contract address from indexer response ({tokenBalances.balances[0].contractAddress}) to match contract address we queried ({token.contractAddress})");
                                }

                                decoratedFeeOptions[i] = new FeeOptionReturn(feeOptions.feeOptions[i],
                                    requiredBalance <= tokenBalances.balances[0].balance);
                            }
                            else
                            {
                                decoratedFeeOptions[i] = new FeeOptionReturn(feeOptions.feeOptions[i], false);
                            }

                            break;
                        case FeeTokenType.erc1155Token:
                            Dictionary<BigInteger, TokenBalance> sftBalances =
                                await indexer.GetTokenBalancesOrganizedInDictionary(_address, token.contractAddress,
                                    false);
                            if (sftBalances.TryGetValue(BigInteger.Parse(token.tokenID), out TokenBalance balance))
                            {
                                decoratedFeeOptions[i] = new FeeOptionReturn(feeOptions.feeOptions[i],
                                    requiredBalance <= balance.balance);
                            }
                            else
                            {
                                decoratedFeeOptions[i] = new FeeOptionReturn(feeOptions.feeOptions[i], false);
                            }

                            break;
                    }
                }

                return new FeeOptionsResponse(decoratedFeeOptions, feeOptions.feeQuote);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to determine which fee option tokens the user has in their wallet: " +
                                    e.Message);
            }
        }

        public async Task<TransactionReturn> SendTransactionWithFeeOptions(Chain network, Transaction[] transactions, FeeOption feeOption, string feeQuote,
            bool waitForReceipt = true, uint timeBeforeExpiry = 30)
        {
            FeeToken token = feeOption.token;
            if (network.GetChainId() != token.chainId.ToString())
            {
                FailedTransactionReturn failedTransactionReturn = new FailedTransactionReturn(
                    $"Failed to send transaction with Fee Options: Specified network ({network}) id {network.GetChainId()} does not match the network id in the {nameof(feeOption)} {token.chainId}",
                    new IntentDataSendTransaction(_address, network, transactions, feeQuote));
                OnSendTransactionFailed?.Invoke(failedTransactionReturn);
                return failedTransactionReturn;
            }

            try
            {
                TransactionsValidator.Validate(transactions);
                int transactionCount = transactions.Length;
                Transaction[] transactionsWithFeeOption = new Transaction[transactionCount + 1];
                transactionsWithFeeOption[0] = feeOption.CreateTransaction();
                for (int i = 0; i < transactionCount; i++)
                {
                    transactionsWithFeeOption[i + 1] = transactions[i];
                }

                IntentDataSendTransaction args =
                    new IntentDataSendTransaction(_address, network, transactionsWithFeeOption, feeQuote);

                return await SendTransactionIntent(args, waitForReceipt, timeBeforeExpiry);
            }
            catch (Exception e)
            {
                FailedTransactionReturn failedTransactionReturn = new FailedTransactionReturn(
                    $"Failed to send transaction with FeeOption [{feeOption}]: {e.Message}",
                    new IntentDataSendTransaction(_address, network, transactions, feeQuote));
                OnSendTransactionFailed?.Invoke(failedTransactionReturn);
                return failedTransactionReturn;
            }
            
        }

        public event Action<IntentResponseSessionAuthProof> OnSessionAuthProofGenerated;
        public event Action<string> OnFailedToGenerateSessionAuthProof;

        public async Task<IntentResponseSessionAuthProof> GetSessionAuthProof(Chain network, string nonce = null)
        {
            IntentDataSessionAuthProof args = new IntentDataSessionAuthProof(network, _address, nonce);
            try
            {
                var result = await _intentSender.SendIntent<IntentResponseSessionAuthProof, IntentDataSessionAuthProof>(args, IntentType.SessionAuthProof);
                OnSessionAuthProofGenerated?.Invoke(result);
                return result;
            }
            catch (Exception e)
            {
                OnFailedToGenerateSessionAuthProof?.Invoke("Failed to generate session auth proof: " + e.Message);
                return null;
            }
        }
        
        public event Action<IntentResponseAccountList> OnAccountListGenerated;
        public event Action<string> OnFailedToGenerateAccountList;
        
        public async Task<IntentResponseAccountList> GetAccountList()
        {
            try
            {
                var result = await _intentSender.SendIntent<IntentResponseAccountList, IntentDataListAccounts>(new IntentDataListAccounts(_address), IntentType.ListAccounts);
                OnAccountListGenerated?.Invoke(result);
                return result;
            }
            catch (Exception e)
            {
                OnFailedToGenerateAccountList?.Invoke("Failed to generate account list: " + e.Message);
                return null;
            }
        }

        public event Action<IntentResponseGetIdToken> OnIdTokenRetrieved;
        public event Action<string> OnFailedToRetrieveIdToken;

        public async Task<IntentResponseGetIdToken> GetIdToken(string nonce = null)
        {
            IntentDataGetIdToken args = new IntentDataGetIdToken(SessionId, _address, nonce);
            try
            {
                var result = await _intentSender.SendIntent<IntentResponseGetIdToken, IntentDataGetIdToken>(args, IntentType.GetIdToken);
                OnIdTokenRetrieved?.Invoke(result);
                return result;
            }

            catch (Exception e)
            {
                OnFailedToRetrieveIdToken?.Invoke("Failed to retrieve Id Token : " + e.Message);
                return null;
            }
        }

        public string GetEmail()
        {
            return _email;
        }

        public event Action<string, bool> OnFederatedAccountRemovedComplete;
        public async Task<bool> RemoveFederatedAccount(Account account)
        {
            IntentDataRemoveAccount args = new IntentDataRemoveAccount(account.wallet, account.id);
            try
            {
                var response =
                    await _intentSender.SendIntent<IntentResponseAccountRemoved, IntentDataRemoveAccount>(args,
                        IntentType.RemoveAccount);
                OnFederatedAccountRemovedComplete?.Invoke(account.id, true);
                return true;
            }
            catch (Exception e)
            {
                OnFederatedAccountRemovedComplete?.Invoke($"Failed to remove federated account with id {account.id}, reason: {e.Message}", false);
                return false;
            }
        }
    }
}