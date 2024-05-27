using System;
using UnityEngine;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.IO;
using System.Net;
using Sequence.Config;

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

        private static readonly Dictionary<string, string> IndexerNames
        = new Dictionary<string, string>
    {
        { Chain.Ethereum.GetChainId(), "mainnet" },
        { Chain.Polygon.GetChainId(), "polygon" },
        { Chain.PolygonZkEvm.GetChainId(), "polygon-zkevm" },
        { Chain.BNBSmartChain.GetChainId(), "bsc" },
        { Chain.ArbitrumOne.GetChainId(), "arbitrum" },
        { Chain.ArbitrumNova.GetChainId(), "arbitrum-nova" },
        { Chain.Optimism.GetChainId(), "optimism" },
        { Chain.Avalanche.GetChainId(), "avalanche" },
        { Chain.Gnosis.GetChainId(), "gnosis" },
        { Chain.Base.GetChainId(), "base" },
        { Chain.OasysHomeverse.GetChainId(), "homeverse" },
        { Chain.AstarZKEvm.GetChainId(), "astar-zkevm" },
        { Chain.Xai.GetChainId(), "xai" },

        { Chain.TestnetSepolia.GetChainId(), "sepolia" },
        { Chain.TestnetArbitrumSepolia.GetChainId(), "arbitrum-sepolia" },
        { Chain.TestnetBNBSmartChain.GetChainId(), "bsc-testnet" },
        { Chain.TestnetBaseSepolia.GetChainId(), "base-sepolia" },
        { Chain.TestnetOasysHomeverse.GetChainId(), "homeverse-testnet" },
        { Chain.TestnetAvalanche.GetChainId(), "avalanche-testnet" },
        { Chain.TestnetOptimisticSepolia.GetChainId(), "optimism-sepolia" },
        { Chain.TestnetPolygonAmoy.GetChainId(), "amoy" }, 
        { Chain.TestnetAstarZKyoto.GetChainId(), "astar-zkyoto" }, 
        { Chain.TestnetXrSepolia.GetChainId(), "xr-sepolia" },
        { Chain.TestnetXaiSepolia.GetChainId(), "xai-sepolia" }, 
    };

        private static string _builderApiKey = SequenceConfig.GetConfig().BuilderAPIKey;
        
        /// <summary>
        /// Combines <see cref="PATH"/> and <paramref name="name"/> to suffix on to the Base Address
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Url(string chainID, string endPoint)
        {
            return $"{HostName(chainID)}{PATH}{endPoint}";
        }

        /// <summary>
        /// Get HostName directing to specific <paramref name="chainID"/>
        /// </summary>
        /// <param name="chainID"></param>
        /// <returns></returns>
        /// <exception>Throws if the chainID isn't a Sequence-supported chain.</exception>
        private static string HostName(string chainID)
        {
            var indexerName = IndexerNames[chainID];
            return $"https://{indexerName}-indexer.sequence.app";
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

        [Obsolete]
        public static async Task<bool> Ping(BigInteger chainID)
        {
            return await Ping(chainID.ToString());
        }
        
        /// <summary>
        /// Retrive indexer status
        /// </summary>
        /// <returns>true if this chain's indexer is good, false otherwise</returns>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<bool> Ping(string chainID)
        {
            string responseBody = await HttpPost(chainID, "Ping", null);
            return BuildResponse<PingReturn>(responseBody).status;
        }

        [Obsolete]
        public static async Task<Version> Version(BigInteger chainID)
        {
            return await Version(chainID.ToString());
        }

        /// <summary>
        /// Retrieve indexer version information.
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<Version> Version(string chainID)
        {
            var responseBody = await HttpPost(chainID, "Version", null);
            return BuildResponse<VersionReturn>(responseBody).version;
        }

        [Obsolete]
        public static async Task<RuntimeStatus> RuntimeStatus(BigInteger chainID)
        {
            return await RuntimeStatus(chainID.ToString());
        }

        /// <summary>
        /// Retrieve indexer runtime status information
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<RuntimeStatus> RuntimeStatus(string chainID)
        {
            var responseBody = await HttpPost(chainID, "RuntimeStatus", null);
            return BuildResponse<RuntimeStatusReturn>(responseBody).status;
        }

        /// <summary>
        /// Retrieve the chain ID for a given BlockChainType
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>

        public static async Task<BigInteger> GetChainID(BigInteger chainID)
        {
            var responseBody = await HttpPost(chainID, "GetChainID", null);
            return BuildResponse<GetChainIDReturn>(responseBody).chainID;
        }

        [Obsolete]
        public static async Task<EtherBalance> GetEtherBalance(BigInteger chainID, string accountAddress)
        {
            return await GetEtherBalance(chainID.ToString(), accountAddress);
        }

        /// <summary>
        /// Retrieve the balance of a network's native token for a given account address
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<EtherBalance> GetEtherBalance(string chainID, string accountAddress)
        {
            var responseBody = await HttpPost(chainID, "GetEtherBalance", new GetEtherBalanceArgs(accountAddress));
            return BuildResponse<GetEtherBalanceReturn>(responseBody).balance;
        }

        [Obsolete]
        public static async Task<GetTokenBalancesReturn> GetTokenBalances(BigInteger chainID, GetTokenBalancesArgs args)
        {
            return await GetTokenBalances(chainID.ToString(), args);
        }

        /// <summary>
        /// Retrieve an account's token balances, optionally for a specific contract
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetTokenBalancesReturn> GetTokenBalances(string chainID, GetTokenBalancesArgs args)
        {
            var responseBody = await HttpPost(chainID, "GetTokenBalances", args);
            return BuildResponse<GetTokenBalancesReturn>(responseBody);
        }

        [Obsolete]
        public static async Task<GetTokenSuppliesReturn> GetTokenSupplies(BigInteger chainID, GetTokenSuppliesArgs args)
        {
            return await GetTokenSupplies(chainID.ToString(), args);
        }

        /// <summary>
        /// Retrieve the token supply for a given contract
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetTokenSuppliesReturn> GetTokenSupplies(string chainID, GetTokenSuppliesArgs args)
        {
            var responseBody = await HttpPost(chainID, "GetTokenSupplies", args);
            return BuildResponse<GetTokenSuppliesReturn>(responseBody);
        }

        [Obsolete]
        public static async Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(BigInteger chainID, GetTokenSuppliesMapArgs args)
        {
            return await GetTokenSuppliesMap(chainID.ToString(), args);
        }

        /// <summary>
        /// Retrieve <see cref="GetTokenSuppliesMapReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(string chainID, GetTokenSuppliesMapArgs args)
        {
            var responseBody = await HttpPost(chainID, "GetTokenSuppliesMap", args);
            return BuildResponse<GetTokenSuppliesMapReturn>(responseBody);
        }

        [Obsolete]
        public static async Task<GetBalanceUpdatesReturn> GetBalanceUpdates(BigInteger chainID, GetBalanceUpdatesArgs args)
        {
            return await GetBalanceUpdates(chainID.ToString(), args);
        }

        /// <summary>
        /// Retrieve <see cref="GetBalanceUpdatesReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetBalanceUpdatesReturn> GetBalanceUpdates(string chainID, GetBalanceUpdatesArgs args)
        {
            var responseBody = await HttpPost(chainID, "GetBalanceUpdates", args);
            return BuildResponse<GetBalanceUpdatesReturn>(responseBody);
        }

        [Obsolete]
        public static async Task<GetTransactionHistoryReturn> GetTransactionHistory(BigInteger chainID, GetTransactionHistoryArgs args)
        {
            return await GetTransactionHistory(chainID.ToString(), args);
        }

        /// <summary>
        /// Retrieve transaction history <see cref="GetTransactionHistoryReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetTransactionHistoryReturn> GetTransactionHistory(string chainID, GetTransactionHistoryArgs args)
        {
            var responseBody = await HttpPost(chainID, "GetTransactionHistory", args);
            return BuildResponse<GetTransactionHistoryReturn>(responseBody);
        }

        [Obsolete]
        private static async Task<string> HttpPost(BigInteger chainID, string endPoint, object args, int retries = 0)
        {
            return await HttpPost(chainID.ToString(), endPoint, args, retries);
        }

        /// <summary>
        /// Makes an HTTP Post Request with content-type set to application/json
        /// </summary>
        /// <returns></returns>
        private static async Task<string> HttpPost(string chainID, string endPoint, object args, int retries = 0)
        {
            if (string.IsNullOrWhiteSpace(_builderApiKey))
            {
                throw SequenceConfig.MissingConfigError("Builder API Key");
            }
            
            string requestJson = JsonConvert.SerializeObject(args);
            using var req = UnityWebRequest.Put(Url(chainID, endPoint), requestJson);
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Accept", "application/json");
            req.SetRequestHeader("X-Access-Key", _builderApiKey); 
            req.method = UnityWebRequest.kHttpVerbPOST;
            req.timeout = 10; // Request will timeout after 10 seconds
                
            string curlRequest = 
                $"curl -X POST -H \"Content-Type: application/json\" -H \"Accept: application/json\" -H \"X-Access-Key: {req.GetRequestHeader("X-Access-Key")}\" -d '{requestJson}' {Url(chainID, endPoint)}";
            try
            {
                await req.SendWebRequest();
                if (req.responseCode < 200 || req.responseCode > 299 || req.error != null ||
                    req.result == UnityWebRequest.Result.ConnectionError ||
                    req.result == UnityWebRequest.Result.ProtocolError)
                {
                    throw new Exception("Failed to make request, non-200 status code " + req.responseCode +
                                        " with error: " + req.error);
                }

                string returnText = req.downloadHandler.text;
                req.Dispose();
                return returnText;
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("HTTP Request failed: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FormatException e)
            {
                Debug.LogError("Invalid URL format: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }
            catch (FileLoadException e)
            {
                if (e.Message.Contains($"{(int)HttpStatusCode.TooManyRequests}"))
                {
                    if (retries == 5)
                    {
                        Debug.LogError("Sequence server rate limit exceeded, giving up after 5 retries..." + "\nCurl-equivalent request: " + curlRequest);
                    }
                    else
                    {
                        
                        Debug.LogWarning($"Sequence server rate limit exceeded, trying again... Retries so far: {retries}" + "\nCurl-equivalent request: " + curlRequest);
                        return await RetryHttpPost(chainID, endPoint, args, 5 * retries, retries);
                    }
                }   
                else
                {
                    Debug.LogWarning("File load exception: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
                }
            }
            catch (Exception e) {
                Debug.LogError("An unexpected error occurred: " + e.Message + "\nCurl-equivalent request: " + curlRequest);
            }

            return "";
        }

        private static async Task<string> RetryHttpPost(string chainID, string endPoint, object args, float waitInSeconds, int retries)
        {
            await AsyncExtensions.DelayTask(waitInSeconds);
            return await HttpPost(chainID, endPoint, args, retries + 1);
        }

        /// <summary>
        /// Parses <paramref name="text"/> into JSON, if it fails due to <paramref name="text"/> not being of JSON format, will throw an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        private static T BuildResponse<T>(string text)
        {
            try
            {
                T data = JsonConvert.DeserializeObject<T>(text);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError("Error building response" + e.Message);
            }
            return default;
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