using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.WaaS.Authentication;

namespace Sequence.WaaS
{   
    public interface IWallet
    {
        public Address GetWalletAddress();
        public event Action<string> OnSignMessageComplete;
        public Task<string> SignMessage(SignMessageArgs args);

        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(IsValidMessageSignatureArgs args);
        public event Action<SuccessfulTransactionReturn> OnSendTransactionComplete;
        public event Action<FailedTransactionReturn> OnSendTransactionFailed;
        public Task<TransactionReturn> SendTransaction(SendTransactionArgs args);
        public event Action<SuccessfulContractDeploymentReturn> OnDeployContractComplete;
        public event Action<FailedContractDeploymentReturn> OnDeployContractFailed;
        public Task<ContractDeploymentReturn> DeployContract(Chain network, string bytecode, string value = "0");
        public event Action<string> OnDropSessionComplete;
        public Task<bool> DropSession(string dropSessionId);
        public Task<bool> DropThisSession();
        public event Action<WaaSSession[]> OnSessionsFound;
        public Task<WaaSSession[]> ListSessions();
    }
}