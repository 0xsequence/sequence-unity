using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Extensions;
using Sequence.Provider;
using UnityEngine;
using static Sequence.Contracts.Contract;

namespace Sequence.Contracts
{
    public class ERC1155 : Ownable
    {
        Contract contract;
        public static readonly string Abi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"values\",\"type\":\"uint256[]\"}],\"name\":\"TransferBatch\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"TransferSingle\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"value\",\"type\":\"string\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"URI\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"accounts\",\"type\":\"address[]\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"}],\"name\":\"balanceOfBatch\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"values\",\"type\":\"uint256[]\"}],\"name\":\"burnBatch\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"exists\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"amounts\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"mintBatch\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"amounts\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeBatchTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"newuri\",\"type\":\"string\"}],\"name\":\"setURI\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"uri\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";

        public ERC1155(Contract contract) : base(contract)
        {
            this.contract = contract;
        }

        public ERC1155(string contractAddress, string abi = null) : base (contractAddress)
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
        public CallContractFunctionTransactionCreator Mint(string toAddress, BigInteger tokenId, BigInteger amount, byte[] data = null)
        {
            if (data == null)
            {
                data = "Minted using the Sequence Unity SDK".ToByteArray(); // Contract expects some data that is non empty
            }
            return contract.CallFunction("mint", toAddress, tokenId, amount, data);
        }

        public CallContractFunctionTransactionCreator MintBatch(string toAddress, BigInteger[] tokenIds, BigInteger[] amounts, byte[] data = null)
        {
            if (data == null)
            {
                data = "Minted using the Sequence Unity SDK".ToByteArray(); // Contract expects some data that is non empty
            }
            return contract.CallFunction("mintBatch", toAddress, tokenIds, amounts, data);
        }
        #endregion

        #region UpdatableURI
        public CallContractFunctionTransactionCreator SetURI(string newURI)
        {
            return contract.CallFunction("setURI", newURI);
        }
        #endregion

        public async Task<BigInteger> BalanceOf(IEthClient client, string address, BigInteger tokenId)
        {
            BigInteger result = await contract.SendQuery<BigInteger>(client, "balanceOf", address, tokenId);
            return result;
        }

        // public async Task<BigInteger[]> BalanceOfBatch(IEthClient client, string[] addresses, BigInteger[] tokenIds)
        // {
        //     string results = await contract.SendQuery(client, "balanceOfBatch", addresses, tokenIds);
        //     return ExtractBigIntegersFromResponse(results);
        // }

        /// <summary>
        /// RPC returns a string array that looks something like this "0x000000211230000000011aa20000000012023" that will be at least 192 characters (96 bytes) long
        /// This function will convert this and return a BigInteger[] for each of the values
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private BigInteger[] ExtractBigIntegersFromResponse(string result)
        {
            result = result.Replace("0x", "");

            int resultLength = result.Length;
            if (resultLength < 192)
            {
                throw new ArgumentOutOfRangeException($"Invalid method input, must be at least 192 characters (96 bytes) long. Input: {result}");
            }

            // Split the string into segments of 64 characters (32 bytes)
            List<string> segments = new List<string>();
            for (int i = 128; i < resultLength; i += 64) // Ignore first 64 bytes - first 32 bytes is an offset for where the array is described, i.e. 32 bytes - next 32 bytes is the size of the array
            {
                segments.Add(result.Substring(i, 64));
            }

            // Convert each segment into a BigInteger
            BigInteger[] results = new BigInteger[segments.Count];
            for (int i = 0; i < segments.Count; i++)
            {
                results[i] = BigInteger.Parse(segments[i], NumberStyles.HexNumber);
            }
            return results;
        }

        public async Task<string> URI(IEthClient client, BigInteger tokenId)
        {
            string result = await contract.SendQuery<string>(client, "uri", tokenId);
            return result;
        }

        public CallContractFunctionTransactionCreator SetApprovalForAll(string operatorAddress, bool approved)
        {
            return contract.CallFunction("setApprovalForAll", operatorAddress, approved);
        }

        public async Task<bool> IsApprovedForAll(IEthClient client, string ownerAddress, string operatorAddress)
        {
            bool isApproved = await contract.SendQuery<bool>(client, "isApprovedForAll", ownerAddress, operatorAddress);
            return isApproved;
        }

        public CallContractFunctionTransactionCreator SafeTransferFrom(string fromAddress, string toAddress, BigInteger tokenId, BigInteger value, byte[] data = null)
        {
            if (data == null)
            {
                data = "Transferred using the Sequence Unity SDK".ToByteArray(); // Contract expects some data that is non empty
            }
            return contract.CallFunction("safeTransferFrom", fromAddress, toAddress, tokenId, value, data);
        }

        public CallContractFunctionTransactionCreator SafeBatchTransferFrom(string fromAddress, string toAddress, BigInteger[] tokenIds, BigInteger[] values, byte[] data = null)
        {
            if (data == null)
            {
                data = "Transferred using the Sequence Unity SDK".ToByteArray(); // Contract expects some data that is non empty
            }
            return contract.CallFunction("safeBatchTransferFrom", fromAddress, toAddress, tokenIds, values, data);
        }

        #region Burnable
        public CallContractFunctionTransactionCreator Burn(string fromAddress, BigInteger tokenId, BigInteger value)
        {
            return contract.CallFunction("burn", fromAddress, tokenId, value);
        }

        public CallContractFunctionTransactionCreator BurnBatch(string fromAddress, BigInteger[] tokenIds, BigInteger[] values)
        {
            return contract.CallFunction("burnBatch", fromAddress, tokenIds, values);
        }
        #endregion

        #region Supply Tracking
        public async Task<BigInteger> TotalSupply(IEthClient client, BigInteger tokenId)
        {
            BigInteger result = await contract.SendQuery<BigInteger>(client, "totalSupply", tokenId);
            return result;
        }

        public async Task<bool> Exists(IEthClient client, BigInteger tokenId)
        {
            bool result = await contract.SendQuery<bool>(client, "exists", tokenId);
            return result;
        }
        #endregion
    }
}
