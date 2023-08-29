using System;
using UnityEngine;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.IO;

namespace Sequence
{
    #region Enums

    public enum ContractType
    {
        UNKNOWN,
        ERC20,
        ERC721,
        ERC1155,
        SEQUENCE_WALLET,
        ERC20_BRIDGE,
        ERC721_BRIDGE,
        ERC1155_BRIDGE
    }

    public enum EventLogType
    {
        UNKNOWN,
        BLOCK_ADDED,
        BLOCK_REMOVED
    }

    public enum EventLogDataType
    {
        UNKNOWN,
        TOKEN_TRANSFER,
        SEQUENCE_TXN
    }

    public enum TxnTransferType
    {
        UNKNOWN,
        SEND,
        RECEIVE
    }

    public enum SortOrder
    {
        DESC,
        ASC
    }
    #endregion

    public static class Indexer
    {
        private const string PATH = "/rpc/Indexer/";

        private static readonly Dictionary<BigInteger, string> IndexerNames
        = new Dictionary<BigInteger, string>
    {
        { (int)Chain.Ethereum, "mainnet" },
        { (int)Chain.Polygon, "polygon" },
        { (int)Chain.PolygonZkEvm, "polygon-zkevm" },
        { (int)Chain.BNBSmartChain, "bsc" },
        { (int)Chain.ArbitrumOne, "arbitrum" },
        { (int)Chain.ArbitrumNova, "arbitrum-nova" },
        { (int)Chain.Optimism, "optimism" },
        { (int)Chain.Avalanche, "avalanche" },
        { (int)Chain.Gnosis, "gnosis" },
        { (int)Chain.Base, "base"},

        { (int)Chain.TestnetGoerli, "goerli" },
        { (int)Chain.TestnetSepolia, "sepolia" },
        { (int)Chain.TestnetPolygonMumbai, "mumbai" },
        { (int)Chain.TestnetArbitrumGoerli, "arbitrum-goerli" },
        { (int)Chain.TestnetBNBSmartChain, "bsc-testnet" },
        { (int)Chain.TestnetBaseGoerli, "base-goerli" },
    };

        /// <summary>
        /// Combines <see cref="PATH"/> and <paramref name="name"/> to suffix on to the Base Address
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Url(BigInteger chainID, string endPoint)
        {
            return $"{HostName(chainID)}{PATH}{endPoint}";
        }

        /// <summary>
        /// Get HostName directing to specific <paramref name="chainID"/>
        /// </summary>
        /// <param name="chainID"></param>
        /// <returns></returns>
        /// <exception>Throws if the chainID isn't a Sequence-supported chain.</exception>
        private static string HostName(BigInteger chainID)
        {
            var indexerName = IndexerNames[chainID];
            return $"https://{indexerName}-indexer-v2.sequence.app";
        }

        public static async Task<T[]> FetchMultiplePages<T>(Func<int, Task<(Page, T[])>> func, int maxPages)
        {
            var nextPage = 0;
            List<T> allItems = new List<T>();
            while (nextPage != -1 && nextPage < maxPages)
            {
                (var page, var items) = await func(nextPage);
                allItems.AddRange(items);

                if (page.more && page.page != 0)
                {
                    nextPage = page.page;
                }
                else { nextPage = -1; }
                // todo add rate limit
            }
            return allItems.ToArray();
        }

        /// <summary>
        /// Retrive indexer status
        /// </summary>
        /// <returns>true if this chain's indexer is good, false otherwise</returns>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<bool> Ping(BigInteger chainID)
        {
            string responseBody = await HTTPPost(chainID, "Ping", null);
            return BuildResponse<PingReturn>(responseBody).status;
        }

        /// <summary>
        /// Retrieve indexer version information.
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<Version> Version(BigInteger chainID)
        {


            var responseBody = await HTTPPost(chainID, "Version", null);
            return BuildResponse<VersionReturn>(responseBody).version;
        }

        /// <summary>
        /// Retrieve indexer runtime status information
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<RuntimeStatus> RuntimeStatus(BigInteger chainID)
        {
            var responseBody = await HTTPPost(chainID, "RuntimeStatus", null);
            return BuildResponse<RuntimeStatusReturn>(responseBody).status;
        }

        /// <summary>
        /// Retrieve the chain ID for a given BlockChainType
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>

        public static async Task<BigInteger> GetChainID(BigInteger chainID)
        {
            var responseBody = await HTTPPost(chainID, "GetChainID", null);
            return BuildResponse<GetChainIDReturn>(responseBody).chainID;
        }

        /// <summary>
        /// Retrieve the balance of a network's native token for a given account address
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<EtherBalance> GetEtherBalance(BigInteger chainID, string accountAddress)
        {
            var responseBody = await HTTPPost(chainID, "GetEtherBalance", new GetEtherBalanceArgs(accountAddress));
            return BuildResponse<GetEtherBalanceReturn>(responseBody).balance;
        }

        /// <summary>
        /// Retrieve an account's token balances, optionally for a specific contract
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetTokenBalancesReturn> GetTokenBalances(BigInteger chainID, GetTokenBalancesArgs args)
        {
            var responseBody = await HTTPPost(chainID, "GetTokenBalances", args);
            return BuildResponse<GetTokenBalancesReturn>(responseBody);
        }

        /// <summary>
        /// Retrieve the token supply for a given contract
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetTokenSuppliesReturn> GetTokenSupplies(BigInteger chainID, GetTokenSuppliesArgs args)
        {
            var responseBody = await HTTPPost(chainID, "GetTokenSupplies", args);
            return BuildResponse<GetTokenSuppliesReturn>(responseBody);
        }

        /// <summary>
        /// Retrieve <see cref="GetTokenSuppliesMapReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(BigInteger chainID, GetTokenSuppliesMapArgs args)
        {
            var responseBody = await HTTPPost(chainID, "GetTokenSuppliesMap", args);
            return BuildResponse<GetTokenSuppliesMapReturn>(responseBody);

        }

        /// <summary>
        /// Retrieve <see cref="GetBalanceUpdatesReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetBalanceUpdatesReturn> GetBalanceUpdates(BigInteger chainID, GetBalanceUpdatesArgs args)
        {
            var responseBody = await HTTPPost(chainID, "GetBalanceUpdates", args);
            return BuildResponse<GetBalanceUpdatesReturn>(responseBody);
        }

        /// <summary>
        /// Retrieve transaction history <see cref="GetTransactionHistoryReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>

        public static async Task<GetTransactionHistoryReturn> GetTransactionHistory(BigInteger chainID, GetTransactionHistoryArgs args)
        {
            var responseBody = await HTTPPost(chainID, "GetTransactionHistory", args);
            return BuildResponse<GetTransactionHistoryReturn>(responseBody);
        }

        /// <summary>
        /// Makes an HTTP Post Request with content-type set to application/json
        /// </summary>
        /// <returns></returns>
        private static async Task<string> HTTPPost(BigInteger chainID, string endPoint, object args)
        {
            var req = UnityWebRequest.Put(Url(chainID, endPoint), JsonConvert.SerializeObject(args));
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Accept", "application/json");
            req.method = UnityWebRequest.kHttpVerbPOST;
            await req.SendWebRequest();
            if (req.responseCode < 200 || req.responseCode > 299)
            {
                throw new System.Exception("Failed to make request, non-200 status code " + req.responseCode);
            }

            string returnText = req.downloadHandler.text;
            req.Dispose();
            return returnText;
        }

        /// <summary>
        /// Parses <paramref name="text"/> into JSON, if it fails due to <paramref name="text"/> not being of JSON format, will throw an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        private static T BuildResponse<T>(string text)
        {
            T data = JsonConvert.DeserializeObject<T>(text);
            return data;
        }
    }

    public static class ExtensionMethods
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }

        public static TaskAwaiter GetAwaiter(this UnityWebRequestAsyncOperation webReqOp)
        {
            var tcs = new TaskCompletionSource<object>();
            webReqOp.completed += obj =>
            {
                {
                    if (webReqOp.webRequest.responseCode != 200)
                    {
                        tcs.SetException(new FileLoadException(webReqOp.webRequest.error));
                    }
                    else
                    {
                        tcs.SetResult(null);
                    }
                }
            };
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}