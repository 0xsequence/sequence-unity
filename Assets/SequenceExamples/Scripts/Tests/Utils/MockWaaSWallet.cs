using System;
using System.Threading.Tasks;
using Sequence;
using Sequence.WaaS;
using Sequence.WaaS.Authentication;

namespace SequenceExamples.Scripts.Tests.Utils
{
    public class MockWaaSWallet : Sequence.WaaS.IWallet
    {
        public static readonly Address TestAddress = new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
        
        public Address GetWalletAddress()
        {
            return TestAddress;
        }

        public event Action<SignMessageReturn> OnSignMessageComplete;
        public Task<SignMessageReturn> SignMessage(SignMessageArgs args)
        {
            throw new NotImplementedException();
        }

        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(IsValidMessageSignatureArgs args)
        {
            throw new NotImplementedException();
        }

        public event Action<SuccessfulTransactionReturn> OnSendTransactionComplete;
        public event Action<FailedTransactionReturn> OnSendTransactionFailed;
        public Task<TransactionReturn> SendTransaction(SendTransactionArgs args)
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

        public event Action<WaaSSession[]> OnSessionsFound;
        public Task<WaaSSession[]> ListSessions()
        {
            throw new NotImplementedException();
        }
    }
}