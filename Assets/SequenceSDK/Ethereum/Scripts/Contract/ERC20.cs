using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Provider;
using UnityEngine;
using Sequence.Extensions;
using System.Numerics;

namespace Sequence.Contracts
{
    public class ERC20 : MonoBehaviour
    {
        Contract contract;

        public ERC20(Contract contract)
        {
            this.contract = contract;
        }

        public ERC20(string contractAddress)
        {
            this.contract = new Contract(contractAddress);
        }

        public async Task<string> Name(IEthClient client)
        {
            string result = await contract.QueryContract(client, "name()");
            return SequenceCoder.HexStringToHumanReadable(result);
        }

        public async Task<string> Symbol(IEthClient client)
        {
            string result = await contract.QueryContract(client, "symbol()");
            return SequenceCoder.HexStringToHumanReadable(result);
        }

        public async Task<BigInteger> TotalSupply(IEthClient client)
        {
            string result = await contract.QueryContract(client, "totalSupply()");
            return result.HexStringToBigInteger();
        }

        public async Task<BigInteger> BalanceOf(IEthClient client, string address)
        {
            string result = await contract.QueryContract(client, "balanceOf(address)", address);
            return result.HexStringToBigInteger();
        }

        public async Task<BigInteger> Allowance(IEthClient client, string ownerAddress, string spenderAddress)
        {
            string result = await contract.QueryContract(client, "allowance(address, address)", ownerAddress, spenderAddress);
            return result.HexStringToBigInteger();
        }
    }
}
