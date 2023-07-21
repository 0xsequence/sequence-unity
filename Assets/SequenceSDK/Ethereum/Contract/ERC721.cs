using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Extensions;
using Sequence.Provider;
using UnityEngine;
using static Sequence.Contracts.Contract;

namespace Sequence.Contracts
{
    public class ERC721 : Ownable
    {
        Contract contract;
        public static readonly string Abi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeMint\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
        
        public ERC721(Contract contract) : base(contract)
        {
            this.contract = contract;
        }

        public ERC721(string contractAddress, string abi = null) : base (contractAddress)
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

        #region Mintable
        public CallContractFunctionTransactionCreator SafeMint(string toAddress, BigInteger tokenId)
        {
            return contract.CallFunction("safeMint", toAddress, tokenId);
        }
        #region AutoIncrementIds
        public CallContractFunctionTransactionCreator SafeMint(string toAddress)
        {
            return contract.CallFunction("safeMint", toAddress);
        }
        #endregion
        #endregion

        public async Task<BigInteger> BalanceOf(IEthClient client, string address)
        {
            string result = await contract.SendQuery(client, "balanceOf", address);
            return result.HexStringToBigInteger();
        }

        public async Task<string> OwnerOf(IEthClient client, BigInteger tokenId)
        {
            string result = await contract.SendQuery(client, "ownerOf", tokenId);
            return result.Replace("0x", "").TrimStart('0').EnsureHexPrefix();
        }

        public async Task<string> Name(IEthClient client)
        {
            string result = await contract.SendQuery(client, "name");
            return SequenceCoder.HexStringToHumanReadable(result);
        }

        public async Task<string> Symbol(IEthClient client)
        {
            string result = await contract.SendQuery(client, "symbol");
            return SequenceCoder.HexStringToHumanReadable(result);
        }

        public async Task<string> TokenURI(IEthClient client, BigInteger tokenId)
        {
            string result = await contract.SendQuery(client, "tokenURI", tokenId);
            return SequenceCoder.HexStringToHumanReadable(result);
        }

        public CallContractFunctionTransactionCreator Approve(string spenderAddress, BigInteger tokenId)
        {
            return contract.CallFunction("approve", spenderAddress, tokenId);
        }

        public async Task<string> GetApproved(IEthClient client, BigInteger tokenId)
        {
            string result = await contract.SendQuery(client, "getApproved", tokenId);
            return result.Replace("0x", "").TrimStart('0').EnsureHexPrefix();
        }

        public CallContractFunctionTransactionCreator SetApprovalForAll(string operatorAddress, bool approved)
        {
            return contract.CallFunction("setApprovalForAll", operatorAddress, approved);
        }

        public async Task<bool> IsApprovedForAll(IEthClient client, string ownerAddress, string operatorAddress)
        {
            string result = await contract.SendQuery(client, "isApprovedForAll", ownerAddress, operatorAddress);
            bool isApproved = result.HexStringToBool();
            return isApproved;
        }

        public CallContractFunctionTransactionCreator TransferFrom(string fromAddress, string toAddress, BigInteger tokenId)
        {
            return contract.CallFunction("transferFrom", fromAddress, toAddress, tokenId);
        }

        public CallContractFunctionTransactionCreator SafeTransferFrom(string fromAddress, string toAddress, BigInteger tokenId, byte[] data = null)
        {
            if (data != null)
            {
                return contract.CallFunction("safeTransferFrom", fromAddress, toAddress, tokenId, data);
            }
            return contract.CallFunction("safeTransferFrom", fromAddress, toAddress, tokenId);
        }

        #region Burnable
        public CallContractFunctionTransactionCreator Burn(BigInteger tokenId)
        {
            return contract.CallFunction("burn", tokenId);
        }
        #endregion
    }
}
