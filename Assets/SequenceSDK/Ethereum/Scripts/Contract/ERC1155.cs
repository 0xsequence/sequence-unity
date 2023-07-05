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

        public ERC1155(Contract contract) : base(contract)
        {
            this.contract = contract;
        }

        public ERC1155(string contractAddress) : base(contractAddress)
        {
            this.contract = new Contract(contractAddress);
        }

        #region Mintable
        public CallContractFunctionTransactionCreator Mint(string toAddress, BigInteger tokenId, BigInteger amount, byte[] data = null)
        {
            if (data == null)
            {
                data = "Minted using the Sequence Unity SDK".ToByteArray(); // Contract expects some data that is non empty
            }
            return contract.CallFunction("mint(address,uint256,uint256,bytes)", toAddress, tokenId, amount, data);
        }

        public CallContractFunctionTransactionCreator MintBatch(string toAddress, BigInteger[] tokenIds, BigInteger[] amounts, byte[] data = null)
        {
            if (data == null)
            {
                data = "Minted using the Unity SDK".ToByteArray(); // Contract expects some data that is non empty
            }
            return contract.CallFunction("mintBatch(address,uint256[],uint256[],bytes)", toAddress, tokenIds, amounts, data);
        }
        #endregion

        #region UpdatableURI
        public CallContractFunctionTransactionCreator SetURI(string newURI)
        {
            return contract.CallFunction("setURI(string)", newURI);
        }
        #endregion

        public async Task<BigInteger> BalanceOf(IEthClient client, string address, BigInteger tokenId)
        {
            string result = await contract.SendQuery(client, "balanceOf(address,uint256)", address, tokenId);
            return result.HexStringToBigInteger();
        }

        public async Task<BigInteger[]> BalanceOfBatch(IEthClient client, string[] addresses, BigInteger[] tokenIds)
        {
            string results = await contract.SendQuery(client, "balanceOfBatch(address[],uint256[])", addresses, tokenIds);
            return ExtractBigIntegersFromResponse(results);
        }

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
                results[i] = BigInteger.Parse("0" + segments[i], NumberStyles.HexNumber);
            }
            return results;
        }

        public async Task<string> URI(IEthClient client, BigInteger tokenId)
        {
            string result = await contract.SendQuery(client, "uri(uint256)", tokenId);
            return SequenceCoder.HexStringToHumanReadable(result);
        }

        public CallContractFunctionTransactionCreator Approve(string spenderAddress, BigInteger tokenId)
        {
            return contract.CallFunction("approve(address,uint256)", spenderAddress, tokenId);
        }

        public async Task<string> GetApproved(IEthClient client, BigInteger tokenId)
        {
            string result = await contract.SendQuery(client, "getApproved(uint256)", tokenId);
            return result.Replace("0x", "").TrimStart('0').EnsureHexPrefix();
        }

        public CallContractFunctionTransactionCreator SetApprovalForAll(string operatorAddress, bool approved)
        {
            return contract.CallFunction("setApprovalForAll(address,bool)", operatorAddress, approved);
        }

        public async Task<bool> IsApprovedForAll(IEthClient client, string ownerAddress, string operatorAddress)
        {
            string result = await contract.SendQuery(client, "isApprovedForAll(address,address)", ownerAddress, operatorAddress);
            bool isApproved = result.HexStringToBool();
            return isApproved;
        }

        public CallContractFunctionTransactionCreator SafeTransferFrom(string fromAddress, string toAddress, BigInteger tokenId, BigInteger value, byte[] data = null)
        {
            if (data == null)
            {
                data = new byte[0];
            }
            return contract.CallFunction("safeTransferFrom(address,address,uint256,uint256,bytes)", fromAddress, toAddress, tokenId, value, data);
        }

        public CallContractFunctionTransactionCreator SafeBatchTransferFrom(string fromAddress, string toAddress, BigInteger[] tokenIds, BigInteger[] values, byte[] data = null)
        {
            if (data == null)
            {
                data = new byte[0];
            }
            return contract.CallFunction("safeBatchTransferFrom(address,address,uint256[],uint256[],bytes)", fromAddress, toAddress, tokenIds, values, data);
        }

        #region Burnable
        public CallContractFunctionTransactionCreator Burn(string fromAddress, BigInteger tokenId, BigInteger value)
        {
            return contract.CallFunction("burn(address,uint256,uint256)", fromAddress, tokenId, value);
        }

        public CallContractFunctionTransactionCreator BurnBatch(string fromAddress, BigInteger[] tokenIds, BigInteger[] values)
        {
            return contract.CallFunction("burnBatch(address,uint256[],uint256[])", fromAddress, tokenIds, values);
        }
        #endregion

        #region Supply Tracking
        public async Task<BigInteger> TotalSupply(IEthClient client, BigInteger tokenId)
        {
            string result = await contract.SendQuery(client, "totalSupply(uint256)", tokenId);
            return result.HexStringToBigInteger();
        }

        public async Task<bool> Exists(IEthClient client, BigInteger tokenId)
        {
            string result = await contract.SendQuery(client, "exists(uint256)", tokenId);
            return result.HexStringToBool();
        }
        #endregion
    }
}
