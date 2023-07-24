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
    public class ERC20 : Ownable
    {
        Contract contract;
        public static readonly string Abi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"burnFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"internalType\":\"uint8\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"subtractedValue\",\"type\":\"uint256\"}],\"name\":\"decreaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"addedValue\",\"type\":\"uint256\"}],\"name\":\"increaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
        
        public ERC20(Contract contract) : base(contract)
        {
            this.contract = contract;
        }

        public ERC20(string contractAddress, string abi = null) : base (contractAddress)
        {
            if (abi == null)
            {
                this.contract = new Contract(contractAddress, Abi);
            }
            else
            {
                this.contract = new Contract(contractAddress, abi);
            }
        }

        public async Task<string> Name(IEthClient client)
        {
            string result = await contract.SendQuery<string>(client, "name");
            return result;
        }

        public async Task<string> Symbol(IEthClient client)
        {
            string result = await contract.SendQuery<string>(client, "symbol");
            return result;
        }

        public async Task<BigInteger> Decimals(IEthClient client)
        {
            BigInteger result = await contract.SendQuery<BigInteger>(client, "decimals");
            return result;
        }

        public async Task<BigInteger> TotalSupply(IEthClient client)
        {
            BigInteger result = await contract.SendQuery<BigInteger>(client, "totalSupply");
            return result;
        }

        public async Task<BigInteger> BalanceOf(IEthClient client, string address)
        {
            BigInteger result = await contract.SendQuery<BigInteger>(client, "balanceOf", address);
            return result;
        }

        public async Task<BigInteger> Allowance(IEthClient client, string ownerAddress, string spenderAddress)
        {
            BigInteger result = await contract.SendQuery<BigInteger>(client, "allowance", ownerAddress, spenderAddress);
            return result;
        }

        public CallContractFunctionTransactionCreator Transfer(string toAddress, BigInteger amount)
        {
            return contract.CallFunction("transfer", toAddress, amount);
        }

        public CallContractFunctionTransactionCreator Approve(string spenderAddress, BigInteger amount)
        {
            return contract.CallFunction("approve", spenderAddress, amount);
        }

        public CallContractFunctionTransactionCreator TransferFrom(string fromAddress, string toAddress, BigInteger amount)
        {
            return contract.CallFunction("transferFrom", fromAddress, toAddress, amount);
        }

        public CallContractFunctionTransactionCreator IncreaseAllowance(string spenderAddress, BigInteger amount)
        {
            return contract.CallFunction("increaseAllowance", spenderAddress, amount);
        }

        public CallContractFunctionTransactionCreator DecreaseAllowance(string spenderAddress, BigInteger amount)
        {
            return contract.CallFunction("decreaseAllowance", spenderAddress, amount);
        }

        #region mintable
        public CallContractFunctionTransactionCreator Mint(string toAddress, BigInteger amount)
        {
            return contract.CallFunction("mint", toAddress, amount);
        }
        #endregion

        #region burnable
        public CallContractFunctionTransactionCreator Burn(BigInteger amount)
        {
            return contract.CallFunction("burn", amount);
        }

        public CallContractFunctionTransactionCreator BurnFrom(string fromAddress, BigInteger amount)
        {
            return contract.CallFunction("burnFrom", fromAddress, amount);
        }
        #endregion
    }
}
