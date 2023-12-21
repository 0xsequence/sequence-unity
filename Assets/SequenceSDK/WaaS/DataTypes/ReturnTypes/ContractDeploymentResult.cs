namespace Sequence.WaaS
{
    public abstract class ContractDeploymentResult
    {
        
    }
    
    public class SuccessfulContractDeploymentResult : ContractDeploymentResult
    {
        public SuccessfulTransactionReturn TransactionReturn;
        public Address DeployedContractAddress;

        public SuccessfulContractDeploymentResult(SuccessfulTransactionReturn transactionReturn, Address deployedContractAddress)
        {
            TransactionReturn = transactionReturn;
            DeployedContractAddress = deployedContractAddress;
        }
    }
    
    public class FailedContractDeploymentResult : ContractDeploymentResult
    {
        public FailedTransactionReturn TransactionReturn;
        public string Error;

        public FailedContractDeploymentResult(FailedTransactionReturn transactionReturn, string error)
        {
            TransactionReturn = transactionReturn;
            Error = error;
        }
    }
}