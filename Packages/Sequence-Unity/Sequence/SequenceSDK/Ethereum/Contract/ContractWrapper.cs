namespace Sequence.Contracts
{
    public abstract class ContractWrapper
    {
        public Contract Contract { get; private set; }

        protected ContractWrapper(Contract contract)
        {
            this.Contract = contract;
        }

        protected ContractWrapper(string contractAddress, string abi)
        {
            this.Contract = new Contract(contractAddress, abi);
        }

        public static implicit operator Address(ContractWrapper contractWrapper)
        {
            return contractWrapper.Contract.GetAddress();
        }
    }
}