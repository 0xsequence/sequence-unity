using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Provider;
using UnityEngine;
using Sequence.Extensions;
using System.Numerics;
using static Sequence.Contracts.Contract;

namespace Sequence.Contracts
{
    public class ERC20
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

        public async Task<BigInteger> Decimals(IEthClient client)
        {
            string result = await contract.QueryContract(client, "decimals()");
            return result.HexStringToBigInteger();
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

        public CallContractFunctionTransactionCreator Transfer(string toAddress, BigInteger amount)
        {
            return contract.CallFunction("transfer(address,uint256)", toAddress, amount);
        }

        public CallContractFunctionTransactionCreator Approve(string spenderAddress, BigInteger amount)
        {
            return contract.CallFunction("approve(address,uint256)", spenderAddress, amount);
        }

        public CallContractFunctionTransactionCreator TransferFrom(string fromAddress, string toAddress, BigInteger amount)
        {
            return contract.CallFunction("transferFrom(address,address,uint256)", fromAddress, toAddress, amount);
        }

        public CallContractFunctionTransactionCreator IncreaseAllowance(string spenderAddress, BigInteger amount)
        {
            return contract.CallFunction("increaseAllowance(address,uint256)", spenderAddress, amount);
        }

        public CallContractFunctionTransactionCreator DecreaseAllowance(string spenderAddress, BigInteger amount)
        {
            return contract.CallFunction("decreaseAllowance(address,uint256)", spenderAddress, amount);
        }

        #region mintable
        public CallContractFunctionTransactionCreator Mint(string toAddress, BigInteger amount)
        {
            return contract.CallFunction("mint(address,uint256)", toAddress, amount);
        }
        #endregion

        #region burnable
        public CallContractFunctionTransactionCreator Burn(BigInteger amount)
        {
            return contract.CallFunction("burn(uint256)", amount);
        }

        public CallContractFunctionTransactionCreator BurnFrom(string fromAddress, BigInteger amount)
        {
            return contract.CallFunction("burnFrom(address,uint256)", fromAddress, amount);
        }
        #endregion

        #region ownable
        public async Task<string> Owner(IEthClient client)
        {
            string result = await contract.QueryContract(client, "owner()");
            return result.Replace("0x", "").TrimStart('0').EnsureHexPrefix();
        }

        public CallContractFunctionTransactionCreator RenounceOwnership()
        {
            return contract.CallFunction("renounceOwnership()");
        }

        public CallContractFunctionTransactionCreator TransferOwnership(string toAddress)
        {
            return contract.CallFunction("transferOwnership(address)", toAddress);
        }
        #endregion
    }
}
