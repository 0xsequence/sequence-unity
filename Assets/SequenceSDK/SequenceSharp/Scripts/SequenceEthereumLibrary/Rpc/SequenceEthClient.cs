using Newtonsoft.Json;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Sequence.RPC
{
    public class SequenceEthClient : IEthClient
    {
        private HttpRpcClient _httpRpcClient;
        private readonly string url = "";
        public SequenceEthClient()
        {
            _httpRpcClient = new HttpRpcClient(url);
        }

        public SequenceEthClient(string _url)
        {
            _httpRpcClient = new HttpRpcClient(_url);
        }
    
        public async Task<BigInteger> BalanceAt(string address, string blockNumber)
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getBalance", new object[] { address, blockNumber});
            //Deserialize
            BigInteger balance = JsonConvert.DeserializeObject<BigInteger>(response.result.ToString());
            return balance;
        }

        public async Task<Block> BlockByHash(string blockHash)
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getBlockByHash", new object[] { blockHash , true });
            //Deserialize
            Block block = JsonConvert.DeserializeObject<Block>(response.result.ToString());
            return block;
        }

        public async Task<Block> BlockByNumber(string blockNumber)
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getBlockByNumber", new object[] { blockNumber, true });
            //Deserialize
            Block block = JsonConvert.DeserializeObject<Block>(response.result.ToString());
            return block;
        }

        public async Task<string> BlockNumber()
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_blockNumber", new object[] {});
            //Deserialize
            string blockNumber = JsonConvert.DeserializeObject<string>(response.result.ToString());
            return blockNumber;
        }

        public async Task<List<Block>> BlockRange(string start = "earliest", string end = "latest", bool? full = true)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getBlockRange", new object[] {start, end, full});
            List<Block> blocks = JsonConvert.DeserializeObject<List<Block>>(response.result.ToString());
            return blocks;
        }

        public async Task<string> CallContract()
        {
            //[FOCUS IMPLEMENTATION]
            var blockNumber = await BlockNumber();
            RpcResponse response = await _httpRpcClient.SendRequest("eth_call", new object[] { blockNumber });
            //Deserialize
            string result = JsonConvert.DeserializeObject<string>(response.result.ToString());
            return result;
        }

        public Task<string> CallContractAtHash()
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> ChainID()
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_chainId", new object[] { });
            //Deserialize
            string chainId = JsonConvert.DeserializeObject<string>(response.result.ToString());
            return chainId;
        }

        public async Task<string> CodeAt(string address, string blockNumber)
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getCode", new object[] { address, blockNumber });
            //Deserialize
            string code = JsonConvert.DeserializeObject<string>(response.result.ToString());
            return code;
        }

        public async Task<BigInteger> EstimateGas(TransactionCall transactionCall, string blockNumber)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_estimateGas", new object[] { transactionCall, blockNumber });
            BigInteger gas = JsonConvert.DeserializeObject<BigInteger>(response.result.ToString());
            return gas;
        }

        public async Task<FeeHistoryResult> FeeHistory(string blockCount, string newestBlock, int? REWARDPERCENTILES)
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_feeHistory", new object[] {blockCount, newestBlock, REWARDPERCENTILES });
            //Deserialize
            FeeHistoryResult feeHistory = JsonConvert.DeserializeObject<FeeHistoryResult>(response.result.ToString());
            return feeHistory;
        }

        public Task<List<Log>> FilterLogs(Filter filter)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Block> HeaderByHash(string blockHash)
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getBlockByHash", new object[] { blockHash, false });
            //Deserialize
            Block block = JsonConvert.DeserializeObject<Block>(response.result.ToString());
            return block;
        }

        public async Task<Block> HeaderByNumber(string blockNumber)
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getBlockByHash", new object[] {  blockNumber, true });
            //Deserialize
            Block block = JsonConvert.DeserializeObject<Block>(response.result.ToString());
            return block;
        }

        public async Task<string> NetworkId()
        {
            RpcResponse response = await _httpRpcClient.SendRequest("net_version", new object[] {});
            string networkId = JsonConvert.DeserializeObject<string>(response.result.ToString());
            return networkId;
        }

        public async Task<BigInteger> NonceAt(string address, string blockNumber)
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getTransactionCount", new object[] { address, blockNumber });
            //Deserialize
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

        internal async Task<string> SendRawTransaction(string signedTransactionData)
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_sendRawTransaction", new object[] { signedTransactionData });
            
            return response.result.ToString();
        }

        public Task<string> SendTransaction(Transaction transaction)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> StorageAt(string address, BigInteger storagePosition, string blockNumber)
        {
            throw new System.NotImplementedException();
        }

        public async Task<BigInteger> SuggestGasPrice()
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_gasPrice", new object[] { });
            //Deserialize
            BigInteger gasPrice = JsonConvert.DeserializeObject<BigInteger>(response.result.ToString());
            return gasPrice;
        }

        public async Task<string> SuggestGasTipCap()
        {
            //[FOCUS IMPLEMENTATION]
            RpcResponse response = await _httpRpcClient.SendRequest("eth_maxPriorityFeePerGas", new object[] { });
            //Deserialize
            string cap = JsonConvert.DeserializeObject<string>(response.result.ToString());
            return cap;
        }

        public Task<SyncStatus> SyncProgress()
        {
            throw new System.NotImplementedException();
        }

        public async Task<Transaction> TransactionByHash(string transactionHash)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getTransactionByHash", new object[] { });
            //Deserialize
            Transaction result = JsonConvert.DeserializeObject<Transaction>(response.result.ToString());
            return result;
        }

        public async Task<BigInteger> TransactionCount(string blockHash)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getBlockTransactionCountByHash", new object[] { blockHash });
            //Deserialize
            BigInteger transactionCount = JsonConvert.DeserializeObject<BigInteger>(response.result.ToString());
            return transactionCount;
        }

        public Task<Transaction> TransactionInBlock(string blockHash, BigInteger transactionIndex)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TransactionReceipt> TransactionReceipt(string transactionHash)
        {
            RpcResponse response = await _httpRpcClient.SendRequest("eth_getTransactionReceipt", new object[] { transactionHash });
            UnityEngine.Debug.Log("Receipt response json :" + response.result.ToString());
            //Deserialize
           TransactionReceipt receipt = JsonConvert.DeserializeObject<TransactionReceipt>(response.result.ToString());
            return receipt;
        }

        public Task<Transaction> TransactionSender(string blockHash, BigInteger transactionIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}