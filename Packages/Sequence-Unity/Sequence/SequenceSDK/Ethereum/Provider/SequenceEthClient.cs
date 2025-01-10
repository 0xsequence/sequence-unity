using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Provider
{
    public class SequenceEthClient : IEthClient
    {
        private IRpcClient _httpRpcClient;
        private readonly string url = "";
        private string chainId = null;

        public SequenceEthClient(Chain chain)
        {
            _httpRpcClient = new HttpRpcClient(NodeGatewayBridge.GetNodeGatewayUrl(chain));
        }
        
        internal SequenceEthClient()
        {
            _httpRpcClient = new HttpRpcClient(url);
        }

        [Obsolete("Use SequenceEthClient(Chain chain) instead. Public nodes often misbehave and can be unreliable; it is highly recommended to use Sequence's node gateway.")]
        public SequenceEthClient(string _url)
        {
            _httpRpcClient = new HttpRpcClient(_url);
        }

        public SequenceEthClient(IRpcClient rpcClient)
        {
            _httpRpcClient = rpcClient;
        }

        /// <summary>
		/// Throws an exception if the response contains an error
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
        private void ThrowIfResponseHasErrors(RpcResponse response)
        {
            if (response.error != null)
            {
                throw new Exception(response.error.Message);
            }
        }
    
        public async Task<BigInteger> BalanceAt(string address, string blockNumber = null)
        {
            if (blockNumber == null)
            {
                blockNumber = await BlockNumber();
            }
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getBalance", new object[] { address, blockNumber});
            ThrowIfResponseHasErrors(response);
            string balanceHex = response.result.ToString();
            BigInteger balance = balanceHex.HexStringToBigInteger();
            return balance;
        }

        public async Task<Block> BlockByHash(string blockHash)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getBlockByHash", new object[] { blockHash , true });
            ThrowIfResponseHasErrors(response);
            Block block = JsonConvert.DeserializeObject<Block>(response.result.ToString());
            return block;
        }

        public async Task<Block> BlockByNumber(string blockNumber)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getBlockByNumber", new object[] { blockNumber, true });
            ThrowIfResponseHasErrors(response);
            Block block = JsonConvert.DeserializeObject<Block>(response.result.ToString());
            return block;
        }

        public async Task<string> BlockNumber()
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_blockNumber", new object[] {});
            ThrowIfResponseHasErrors(response);
            string blockNumber = JsonConvert.DeserializeObject<string>(response.result.ToString());
            return blockNumber;
        }

        public async Task<List<Block>> BlockRange(string start = "earliest", string end = "earliest")
        {
            if (start == "earliest") {
                start = "0x0";
            }
            if (end == "latest") {
                end = await BlockNumber();
            }

            BigInteger startInt = start.HexStringToBigInteger();
            BigInteger endInt = end.HexStringToBigInteger();

            List<Block> blocks = new List<Block>();
            for (BigInteger i = startInt; i <= endInt; i++)
            {
                Block block = await BlockByNumber(i.BigIntegerToHexString());
                blocks.Add(block);
            }

            return blocks;
        }

        public async Task<string> CallContract(params object[] args)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_call", args.AppendArray(new object[]{"latest"}));
            ThrowIfResponseHasErrors(response);
            string result = response.result.ToString();
            return result;
        }

        public Task<string> CallContractAtHash()
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> ChainID()
        {
            if (chainId == null)
            {
                RpcResponse response = await _httpRpcClient.SendRequest("eth_chainId", new object[] { });
                ThrowIfResponseHasErrors(response);
                chainId = JsonConvert.DeserializeObject<string>(response.result.ToString());
            }
            return chainId;
        }

        public async Task<string> CodeAt(string address, string blockNumber = "latest")
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getCode", new object[] { address, blockNumber });
            ThrowIfResponseHasErrors(response);
            string code = response.result.ToString();
            return code;
        }

        public async Task<BigInteger> EstimateGas(TransactionCall transactionCall)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["from"] = transactionCall.from,
                ["to"] = transactionCall.to,
                ["value"] = transactionCall.value.BigIntegerToHexString(),
                ["data"] = transactionCall.data
            };
            RpcResponse response = await _httpRpcClient.SendRequest("eth_estimateGas", new object[] { parameters });
            ThrowIfResponseHasErrors(response);
            BigInteger gas = JsonConvert.DeserializeObject<BigInteger>(response.result.ToString());
            return gas;
        }

        public async Task<FeeHistoryResult> FeeHistory(string blockCount, string newestBlock, int[] REWARDPERCENTILES)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_feeHistory", new object[] {blockCount, newestBlock, REWARDPERCENTILES });
            ThrowIfResponseHasErrors(response);
            FeeHistoryResult feeHistory = JsonConvert.DeserializeObject<FeeHistoryResult>(response.result.ToString());
            return feeHistory;
        }

        public Task<List<Log>> FilterLogs(Filter filter)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Block> HeaderByHash(string blockHash)
        {
            return await BlockByHash(blockHash);
        }

        public async Task<Block> HeaderByNumber(string blockNumber)
        {
            return await BlockByNumber(blockNumber);
        }

        public async Task<string> NetworkId()
        {
            RpcResponse response = await _httpRpcClient.SendRequest("net_version", new object[] {});
            ThrowIfResponseHasErrors(response);
            string networkId = JsonConvert.DeserializeObject<string>(response.result.ToString());
            return networkId;
        }

        public async Task<BigInteger> NonceAt(string address, string blockNumber = "latest")
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getTransactionCount", new object[] { address, blockNumber });
            ThrowIfResponseHasErrors(response);
            BigInteger transactionCount = JsonConvert.DeserializeObject<BigInteger>(response.result.ToString());
            return transactionCount;
        }

        public Task<BigInteger> PeerCount()
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> PendingBalanceAt(string address)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> PendingCallContract(TransactionCall transactionCall)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> PendingCodeAt(string address)
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> PendingNonceAt(string address)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> PendingStorageAt(string address, BigInteger storagePosition)
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> PendingTransactionCount()
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> SendRawTransaction(string signedTransactionData)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_sendRawTransaction", new object[] { signedTransactionData });
            ThrowIfResponseHasErrors(response);
            return response.result.ToString();
        }

        public Task<string> SendTransaction(ClientTransaction clientTransaction)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> StorageAt(string address, BigInteger storagePosition, string blockNumber)
        {
            throw new System.NotImplementedException();
        }

        public async Task<BigInteger> SuggestGasPrice()
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_gasPrice", new object[] { });
            ThrowIfResponseHasErrors(response);
            BigInteger gasPrice = JsonConvert.DeserializeObject<BigInteger>(response.result.ToString());
            return gasPrice;
        }

        public async Task<BigInteger> SuggestGasTipCap()
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_maxPriorityFeePerGas", new object[] { });
            ThrowIfResponseHasErrors(response);
            BigInteger cap = JsonConvert.DeserializeObject<BigInteger>(response.result.ToString());
            return cap;
        }

        public Task<SyncStatus> SyncProgress()
        {
            throw new System.NotImplementedException();
        }

        public async Task<ClientTransaction> TransactionByHash(string transactionHash)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getTransactionByHash", new object[] { transactionHash });
            ThrowIfResponseHasErrors(response);
            ClientTransaction result = JsonConvert.DeserializeObject<ClientTransaction>(response.result.ToString());
            return result;
        }

        public async Task<BigInteger> TransactionCount(string blockHash)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getBlockTransactionCountByHash", new object[] { blockHash });
            ThrowIfResponseHasErrors(response);
            BigInteger transactionCount = JsonConvert.DeserializeObject<BigInteger>(response.result.ToString());
            return transactionCount;
        }

        public Task<ClientTransaction> TransactionInBlock(string blockHash, BigInteger transactionIndex)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TransactionReceipt> TransactionReceipt(string transactionHash)
        {
            TransactionReceipt receipt = null;
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getTransactionReceipt", new object[] { transactionHash });
            if (response.error != null && response.error.Message != Errors.NotFound)
            {
                ThrowIfResponseHasErrors(response);
            }
            if (response.result != null)
            {
                receipt = JsonConvert.DeserializeObject<TransactionReceipt>(response.result.ToString());
            }
            return receipt;
        }

        public Task<ClientTransaction> TransactionSender(string blockHash, BigInteger transactionIndex)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TransactionReceipt> WaitForTransactionReceipt(string transactionHash, int maxWaitTimeInMilliseconds = 15000, int timeBetweenChecksInMilliseconds = 500)
        {
            TransactionReceipt receipt = null;
            float startTime = Time.time;
            while (receipt == null)
            {
                receipt = await TransactionReceipt(transactionHash);

                float elapsedTime = Time.time - startTime;
                if (elapsedTime * 1000 + timeBetweenChecksInMilliseconds >= maxWaitTimeInMilliseconds)
                {
                    return receipt;
                }

                Thread.Sleep(timeBetweenChecksInMilliseconds);
            }
            return receipt;
        }
    }
}