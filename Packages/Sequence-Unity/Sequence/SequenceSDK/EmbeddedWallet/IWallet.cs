using System;
using System.Threading.Tasks;

namespace Sequence.EmbeddedWallet
{   
    public interface IWallet
    {
        public Address GetWalletAddress();
        public event Action<string> OnSignMessageComplete;
        public event Action<string> OnSignMessageFailed;

        /// <summary>
        /// Sign the specified message on the specified network
        /// Can be awaited directly and/or you can subscribe to the OnSignMessageComplete method
        ///
        /// Optionally specify a time before the intent sent to the API will be considered invalid - defaults to 30 seconds
        /// </summary>
        /// <param name="network"></param>
        /// <param name="message"></param>
        /// <param name="timeBeforeExpiry"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <returns></returns>
        public Task<string> SignMessage(Chain network, string message, uint timeBeforeExpiry = 30,
            Action<string> onSuccess = null, Action<string> onFail = null);

        /// <summary>
        /// Determine if the specified message and signature are validly signed for the given network by this wallet
        /// </summary>
        /// <param name="network"></param>
        /// <param name="message"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(Chain network, string message, string signature);
        public event Action<SuccessfulTransactionReturn> OnSendTransactionComplete;
        public event Action<FailedTransactionReturn> OnSendTransactionFailed;
        
        /// <summary>
        /// Send an array of Transactions from this wallet on the given Chain
        /// Can be awaited directly and/or you can subscribe to the OnSendTransactionComplete and OnSendTransactionFailed
        /// events to retrieve the SuccessfulTransactionReturn and FailedTransactionReturn responses respectively
        ///
        /// Optionally specify whether you want to waitForReceipt in the event that the API times out while waiting for the TransactionReceipt
        /// In this case, the SDK will continually attempt to fetch the TransactionReceipt
        ///
        /// Optionally specify a time before the intent sent to the API will be considered invalid - defaults to 30 seconds
        /// </summary>
        /// <param name="network"></param>
        /// <param name="transactions"></param>
        /// <param name="waitForReceipt"></param>
        /// <param name="timeBeforeExpiry"></param>
        /// <returns></returns>
        public Task<TransactionReturn> SendTransaction(Chain network, Transaction[] transactions, bool waitForReceipt = true, uint timeBeforeExpiry = 30);
        public event Action<SuccessfulContractDeploymentReturn> OnDeployContractComplete;
        public event Action<FailedContractDeploymentReturn> OnDeployContractFailed;
        
        /// <summary>
        /// Deploy a smart contract (via a compiled bytecode string) to the specified Chain
        /// Can be awaited directly and/or you can subscribe to the OnDeployContractComplete and OnDeployContractFailed events
        /// to receive the SuccessfulContractDeploymentReturn and FailedContractDeploymentReturn responses respectively
        /// </summary>
        /// <param name="network"></param>
        /// <param name="bytecode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<ContractDeploymentReturn> DeployContract(Chain network, string bytecode, string value = "0");
        public event Action<string> OnDropSessionComplete;
        
        /// <summary>
        /// Drop the specified session id (dropSessionId) with the API
        ///
        /// Can be awaited directly and/or you can subscribe to the OnDropSessionComplete event
        /// </summary>
        /// <param name="dropSessionId"></param>
        /// <returns></returns>
        public Task<bool> DropSession(string dropSessionId);
        
        /// <summary>
        /// Drop this session with the API, logging the user out and rendering the wallet instance unusable
        ///
        /// Can be awaited directly and/or you can subscribe to the OnDropSessionComplete event
        /// </summary>
        /// <returns></returns>
        public Task<bool> DropThisSession();
        public event Action<Session[]> OnSessionsFound;
        
        /// <summary>
        /// Find all active sessions registered with the API associated with this user
        ///
        /// Can be awaited directly and/or you can subscribe to the OnSessionsFound event
        /// </summary>
        /// <returns></returns>
        public Task<Session[]> ListSessions();
        
        /// <summary>
        /// Continually poll the API for a TransactionReceipt
        /// Use this if you received a SuccessfulTransactionReturn object that did not have a TransactionReceipt included
        /// This should only be needed if you have specified `waitForReceipt = false` when calling SendTransaction
        /// </summary>
        /// <param name="successfulTransactionReturn"></param>
        /// <returns></returns>
        public Task<SuccessfulTransactionReturn> WaitForTransactionReceipt(
            SuccessfulTransactionReturn successfulTransactionReturn);
        
        /// <summary>
        /// If you do not want to sponsor your user's gas, use this method to retrieve a feeQuote and an array of FeeOptions for the user
        /// Once you've awaited a response for this Task, call SendTransactionWithFeeOptions and include the feeQuote string and selected FeeOption
        ///
        /// Optionally specify a time before the intent sent to the API will be considered invalid - defaults to 30 seconds
        /// </summary>
        /// <param name="network"></param>
        /// <param name="transactions"></param>
        /// <param name="timeBeforeExpiry"></param>
        /// <returns></returns>
        public Task<FeeOptionsResponse> GetFeeOptions(Chain network, Transaction[] transactions, uint timeBeforeExpiry = 30);

        /// <summary>
        /// Send a transaction where the user is paying the transaction fee as specified in the FeeOption
        /// You must call GetFeeOptions with the Chain and Transaction[] first in order to retrieve the FeeOptions and feeQuote
        ///
        /// Can be awaited directly and/or you can subscribe to the OnSendTransactionComplete and OnSendTransactionFailed
        /// events to retrieve the SuccessfulTransactionReturn and FailedTransactionReturn responses respectively
        ///
        /// Optionally specify whether you want to waitForReceipt in the event that the API times out while waiting for the TransactionReceipt
        /// In this case, the SDK will continually attempt to fetch the TransactionReceipt
        ///
        /// Optionally specify a time before the intent sent to the API will be considered invalid - defaults to 30 seconds
        /// </summary>
        /// <param name="network"></param>
        /// <param name="transactions"></param>
        /// <param name="feeOption"></param>
        /// <param name="feeQuote"></param>
        /// <param name="waitForReceipt"></param>
        /// <param name="timeBeforeExpiry"></param>
        /// <returns></returns>
        public Task<TransactionReturn> SendTransactionWithFeeOptions(Chain network, Transaction[] transactions,
            FeeOption feeOption, string feeQuote, bool waitForReceipt = true, uint timeBeforeExpiry = 30);
        
        public event Action<IntentResponseSessionAuthProof> OnSessionAuthProofGenerated;
        public event Action<string> OnFailedToGenerateSessionAuthProof;

        /// <summary>
        /// Get a valid session auth proof for this session and (optional) nonce
        ///
        /// Can be awaited directly or you can subscribe to the OnSessionAuthProofGenerated and OnFailedToGenerateSessionAuthProof events to get success and failed responses respectively
        /// </summary>
        /// <param name="network"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        public Task<IntentResponseSessionAuthProof> GetSessionAuthProof(Chain network, string nonce = null);
        
        public event Action<IntentResponseAccountList> OnAccountListGenerated;
        public event Action<string> OnFailedToGenerateAccountList;
        
        /// <summary>
        /// Get a list of Accounts associated with this wallet
        ///
        /// Can be awaited directly or you can subscribe to the OnAccountListGenerated and OnFailedToGenerateAccountList events to get success and failed responses respectively
        /// </summary>
        /// <returns></returns>
        public Task<IntentResponseAccountList> GetAccountList();

        public event Action<IntentResponseGetIdToken> OnIdTokenRetrieved;
        public event Action<string> OnFailedToRetrieveIdToken;

        public Task<IntentResponseGetIdToken> GetIdToken(string nonce = null);
    }
}