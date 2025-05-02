using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Provider;

namespace Sequence.Contracts
{
    public class MerkleProvable : ContractWrapper
    {
        public Contract Contract { get; private set; }
        public const string Abi = "[{\"type\":\"function\",\"name\":\"checkMerkleProof\",\"inputs\":[{\"name\":\"root\",\"type\":\"bytes32\",\"internalType\":\"bytes32\"},{\"name\":\"proof\",\"type\":\"bytes32[]\",\"internalType\":\"bytes32[]\"},{\"name\":\"addr\",\"type\":\"address\",\"internalType\":\"address\"},{\"name\":\"salt\",\"type\":\"bytes32\",\"internalType\":\"bytes32\"}],\"outputs\":[{\"name\":\"\",\"type\":\"bool\",\"internalType\":\"bool\"}],\"stateMutability\":\"view\"},{\"type\":\"error\",\"name\":\"MerkleProofInvalid\",\"inputs\":[{\"name\":\"root\",\"type\":\"bytes32\",\"internalType\":\"bytes32\"},{\"name\":\"proof\",\"type\":\"bytes32[]\",\"internalType\":\"bytes32[]\"},{\"name\":\"addr\",\"type\":\"address\",\"internalType\":\"address\"},{\"name\":\"salt\",\"type\":\"bytes32\",\"internalType\":\"bytes32\"}]}]";
        
        public MerkleProvable(Contract contract) : base(contract)
        {
            this.Contract = contract;
        }

        public MerkleProvable(string contractAddress, string abi = null) : base(contractAddress, abi)
        {
            if (abi == null)
            {
                this.Contract = new Contract(contractAddress, Abi);
            }
            else
            {
                this.Contract = new Contract(contractAddress, abi);
            }
        }

        public async Task<bool> CheckMerkleProofAsync(IEthClient client, FixedByte root, FixedByte[] proof, string address, FixedByte salt)
        {
            return await Contract.SendQuery<bool>(client, "checkMerkleProof", root, proof, address, salt);
        }
    }
}