using System;
using System.Threading.Tasks;
using Sequence.WaaS.Authentication;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{   
    public interface IWallet
    {
        public Address GetWalletAddress();
        public event Action<string> OnSignMessageComplete;
        public Task<string> SignMessage(Chain network, string message, uint timeBeforeExpiry = 30);

        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(Chain network, string message, string signature);
        public event Action<SuccessfulTransactionReturn> OnSendTransactionComplete;
        public event Action<FailedTransactionReturn> OnSendTransactionFailed;
        public Task<TransactionReturn> SendTransaction(Chain network, Transaction[] transactions, bool waitForReceipt = true, uint timeBeforeExpiry = 30);
        public event Action<SuccessfulContractDeploymentReturn> OnDeployContractComplete;
        public event Action<FailedContractDeploymentReturn> OnDeployContractFailed;
        public Task<ContractDeploymentReturn> DeployContract(Chain network, string bytecode, string value = "0");
        public event Action<string> OnDropSessionComplete;
        public Task<bool> DropSession(string dropSessionId);
        public Task<bool> DropThisSession();
        public event Action<WaaSSession[]> OnSessionsFound;
        public Task<WaaSSession[]> ListSessions();
        public Task<SuccessfulTransactionReturn> WaitForTransactionReceipt(
            SuccessfulTransactionReturn successfulTransactionReturn);
        public Task<FeeOptionsResponse> GetFeeOptions(Chain network, Transaction[] transactions, uint timeBeforeExpiry = 30);
    }
}