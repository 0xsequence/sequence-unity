namespace Sequence.WaaS
{
    public abstract class ContractDeploymentReturn
    {
        
    }
    
    public class SuccessfulContractDeploymentReturn : ContractDeploymentReturn
    {
        public SuccessfulTransactionReturn TransactionReturn;
        public Address DeployedContractAddress;

        public SuccessfulContractDeploymentReturn(SuccessfulTransactionReturn transactionReturn, Address deployedContractAddress)
        {
            TransactionReturn = transactionReturn;
            DeployedContractAddress = deployedContractAddress;
        }
    }
    
    public class FailedContractDeploymentReturn : ContractDeploymentReturn
    {
        public FailedTransactionReturn TransactionReturn;
        public string Error;

        public FailedContractDeploymentReturn(FailedTransactionReturn transactionReturn, string error)
        {
            TransactionReturn = transactionReturn;
            Error = error;
        }
    }
}