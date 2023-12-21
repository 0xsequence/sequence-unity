using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Provider;
using Sequence.Wallet;
using UnityEngine;
using Sequence.Transactions;
using Sequence.Utils;

namespace Sequence.Contracts
{
    public static class ContractDeployer
    {
        public static async Task<ContractDeploymentResult> Deploy(
            IEthClient client,
            IWallet wallet,
            string bytecode,
            BigInteger? gasPrice = null,
            BigInteger? gasLimit = null)
        {
            TransactionReceipt receipt = await wallet.DeployContract(client, bytecode); 
            ContractDeploymentResult result = new ContractDeploymentResult(receipt);
            return result;
        }

        public static string CalculateContractAddress(BigInteger nonce, string senderAddress)
        {
            byte[] addressBytes = SequenceCoder.HexStringToByteArray(senderAddress);
            byte[] nonceBytes = nonce.ToByteArray(true, true);
            List<object> toEncode = new List<object>();
            toEncode.Add(addressBytes);
            toEncode.Add(nonceBytes);
            byte[] encoded = RLP.RLP.Encode(toEncode);
            byte[] hashed = SequenceCoder.KeccakHash(encoded);
            string hashedString = SequenceCoder.ByteArrayToHexString(hashed).EnsureHexPrefix();
            string address = hashedString.Substring(hashedString.Length - 40, 40).EnsureHexPrefix();
            Debug.Log($"Deployer {senderAddress}, nonce {nonce} - deployed to {address}");
            return address;
        }
    }

    public class ContractDeploymentResult
    {
        public string TransactionHash;
        public TransactionReceipt Receipt;
        public Address DeployedContractAddress;

        public ContractDeploymentResult(TransactionReceipt receipt)
        {
            this.TransactionHash = receipt.transactionHash;
            this.Receipt = receipt;
            this.DeployedContractAddress = new Address(receipt.contractAddress);
        }
    }
}
