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

        public ERC721(Contract contract) : base(contract)
        {
            this.contract = contract;
        }

        public ERC721(string contractAddress) : base (contractAddress)
        {
            this.contract = new Contract(contractAddress);
        }

        #region Mintable
        public CallContractFunctionTransactionCreator SafeMint(string toAddress)
        {
            return contract.CallFunction("safeMint(address)", toAddress);
        }
        #endregion

        public async Task<BigInteger> BalanceOf(IEthClient client, string address)
        {
            string result = await contract.SendQuery(client, "balanceOf(address)", address);
            return result.HexStringToBigInteger();
        }

        public async Task<string> OwnerOf(IEthClient client, BigInteger tokenId)
        {
            string result = await contract.SendQuery(client, "ownerOf(uint64)", tokenId);
            return result.Replace("0x", "").TrimStart('0').EnsureHexPrefix();
        }

        public async Task<string> Name(IEthClient client)
        {
            string result = await contract.SendQuery(client, "name()");
            return SequenceCoder.HexStringToHumanReadable(result);
        }

        public async Task<string> Symbol(IEthClient client)
        {
            string result = await contract.SendQuery(client, "symbol()");
            return SequenceCoder.HexStringToHumanReadable(result);
        }

        public async Task<string> TokenURI(IEthClient client, BigInteger tokenId)
        {
            string result = await contract.SendQuery(client, "tokenURI(uint256)", tokenId);
            return SequenceCoder.HexStringToHumanReadable(result);
        }

        public async Task<string> BaseURI(IEthClient client)
        {
            string result = await contract.SendQuery(client, "baseURI()");
            return SequenceCoder.HexStringToHumanReadable(result);
        }

        public CallContractFunctionTransactionCreator Approve(string spenderAddress, BigInteger tokenId)
        {
            return contract.CallFunction("approve(address,uint256)", spenderAddress, tokenId);
        }

        public async Task<string> GetApproved(IEthClient client, BigInteger tokenId)
        {
            string result = await contract.SendQuery(client, "getApproved(uint256)", tokenId);
            return SequenceCoder.HexStringToHumanReadable(result);
        }

        public CallContractFunctionTransactionCreator SetApprovalForAll(string operatorAddress, bool approved)
        {
            return contract.CallFunction("setApprovalForAll(address,bool)", operatorAddress, approved);
        }

        public async Task<bool> IsApprovedForAll(IEthClient client, string operatorAddress)
        {
            string result = await contract.SendQuery(client, "isApprovedForAll(address)", operatorAddress);
            bool isApproved = bool.Parse(result);
            return isApproved;
        }

        public CallContractFunctionTransactionCreator TransferFrom(string fromAddress, string toAddress, BigInteger tokenId)
        {
            return contract.CallFunction("transferFrom(address,address,uint256)", fromAddress, toAddress, tokenId);
        }

        public CallContractFunctionTransactionCreator SafeTransferFrom(string fromAddress, string toAddress, BigInteger tokenId, byte[] data = default)
        {
            return contract.CallFunction("safeTransferFrom(address,address,uint256,bytes)", fromAddress, toAddress, tokenId, data);
        }

        #region Burnable
        public CallContractFunctionTransactionCreator Burn(BigInteger tokenId)
        {
            return contract.CallFunction("burn(uint256)", tokenId);
        }
        #endregion
    }
}
