using System;
using System.Threading.Tasks;
using Sequence;
using Sequence.EmbeddedWallet;
using Sequence.Wallet;

namespace Sequence.EmbeddedWallet
{
    public class MockWaaSWallet : IWallet
    {
        public static readonly Address TestAddress = new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
        
        public Address GetWalletAddress()
        {
            return TestAddress;
        }

        public event Action<string> OnSignMessageComplete;
        public event Action<string> OnSignMessageFailed;


        public Task<string> SignMessage(Chain network, string message, uint timeBeforeExpiry = 30)
        {
            return new EOAWallet().SignMessage(message, ChainDictionaries.ChainIdOf[network]);
        }

        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(Chain network, string message, string signature)
        {
            throw new NotImplementedException();
        }

        public event Action<SuccessfulTransactionReturn> OnSendTransactionComplete;
        public event Action<FailedTransactionReturn> OnSendTransactionFailed;

        public Task<TransactionReturn> SendTransaction(Chain network, Transaction[] transactions, bool waitForReceipt = true, uint timeBeforeExpiry = 30)
        {
            throw new NotImplementedException();
        }

        public event Action<SuccessfulContractDeploymentReturn> OnDeployContractComplete;
        public event Action<FailedContractDeploymentReturn> OnDeployContractFailed;

        public Task<ContractDeploymentReturn> DeployContract(Chain network, string bytecode, string value = "0")
        {
            throw new NotImplementedException();
        }

        public event Action<string> OnDropSessionComplete;
        public Task<bool> DropSession(string dropSessionId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DropThisSession()
        {
            throw new NotImplementedException();
        }

        public event Action<Session[]> OnSessionsFound;
        public Task<Session[]> ListSessions()
        {
            throw new NotImplementedException();
        }

        public Task<SuccessfulTransactionReturn> WaitForTransactionReceipt(SuccessfulTransactionReturn successfulTransactionReturn)
        {
            throw new NotImplementedException();
        }

        public Task<FeeOptionsResponse> GetFeeOptions(Chain network, Transaction[] transactions, uint timeBeforeExpiry = 30)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionReturn> SendTransactionWithFeeOptions(Chain network, Transaction[] transactions, FeeOption feeOption, string feeQuote,
            bool waitForReceipt = true, uint timeBeforeExpiry = 30)
        {
            throw new NotImplementedException();
        }

        public event Action<IntentResponseSessionAuthProof> OnSessionAuthProofGenerated;
        public event Action<string> OnFailedToGenerateSessionAuthProof;

        public Task<IntentResponseSessionAuthProof> GetSessionAuthProof(Chain network, string nonce = null)
        {
            throw new NotImplementedException();
        }

        public event Action<IntentResponseAccountList> OnAccountListGenerated;
        public event Action<string> OnFailedToGenerateAccountList;
        public Task<IntentResponseAccountList> GetAccountList()
        {
            throw new NotImplementedException();
        }

        public event Action<IntentResponseGetIdToken> OnIdTokenRetrieved;
        public event Action<string> OnFailedToRetrieveIdToken;

        public Task<IntentResponseGetIdToken> GetIdToken(string nonce = null)
        {
            throw new NotImplementedException();
        }

        public string GetEmail()
        {
            throw new NotImplementedException();
        }

        public event Action<string, bool> OnFederatedAccountRemovedComplete;
        public Task<bool> RemoveFederatedAccount(Account account)
        {
            throw new NotImplementedException();
        }
    }
}