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
using Sequence.Provider;
using Sequence.Utils;

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
        ERC1155_BRIDGE,
        NATIVE,
        SEQ_MARKETPLACE
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
        EVENT,
        TOKEN_TRANSFER,
        SEQUENCE_TXN,
        NATIVE_TOKEN_TRANSFER
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
        internal const string PATH = "/rpc/Indexer/";

        private static string _builderApiKey = SequenceConfig.GetConfig(SequenceService.Indexer).BuilderAPIKey;
        
        public static Action<string> OnQueryFailed;
        public static Action<string> OnQueryIssue;

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
        public static async Task<bool> Ping(string chainID, IHttpHandler httpHandler = null, IIndexer caller = null)
        {
            string responseBody = await HttpPost(chainID, "Ping", null, 0, httpHandler, caller);
            PingReturn result = BuildResponse<PingReturn>(responseBody, caller);
            if (result != default)
            {
                return result.status;
            }

            return default;
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
        public static async Task<Version> Version(string chainID, IHttpHandler httpHandler = null, IIndexer caller = null)
        {
            var responseBody = await HttpPost(chainID, "Version", null, 0 , httpHandler, caller);
            VersionReturn result = BuildResponse<VersionReturn>(responseBody, caller);
            if (result != default)
            {
                return result.version;
            }

            return default;
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
        public static async Task<RuntimeStatus> RuntimeStatus(string chainID, IHttpHandler httpHandler = null, IIndexer caller = null)
        {
            var responseBody = await HttpPost(chainID, "RuntimeStatus", null, 0, httpHandler, caller);
            RuntimeStatusReturn result = BuildResponse<RuntimeStatusReturn>(responseBody, caller);
            if (result != null)
            {
                return result.status;
            }

            return default;
        }

        /// <summary>
        /// Retrieve the chain ID for a given BlockChainType
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<BigInteger> GetChainID(BigInteger chainID)
        {
            var responseBody = await HttpPost(chainID, "GetChainID", null);
            GetChainIDReturn result = BuildResponse<GetChainIDReturn>(responseBody);
            if (result != default)
            {
                return result.chainID;
            }

            return default;
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
        public static async Task<EtherBalance> GetEtherBalance(string chainID, string accountAddress, int retries = 0, IHttpHandler httpHandler = null, IIndexer caller = null)
        {
            var responseBody = await HttpPost(chainID, "GetEtherBalance", new GetEtherBalanceArgs(accountAddress), retries, httpHandler, caller);
            GetEtherBalanceReturn result = BuildResponse<GetEtherBalanceReturn>(responseBody, caller);
            if (result != default)
            {
                return result.balance;
            }

            return default;
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
        public static async Task<GetTokenBalancesReturn> GetTokenBalances(string chainID, GetTokenBalancesArgs args, int retries = 0, IHttpHandler httpHandler = null, IIndexer caller = null)
        {
            var responseBody = await HttpPost(chainID, "GetTokenBalances", args, retries, httpHandler, caller);
            return BuildResponse<GetTokenBalancesReturn>(responseBody, caller);
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
        public static async Task<GetTokenSuppliesReturn> GetTokenSupplies(string chainID, GetTokenSuppliesArgs args, int retries = 0, IHttpHandler httpHandler = null, IIndexer caller = null)
        {
            var responseBody = await HttpPost(chainID, "GetTokenSupplies", args, retries, httpHandler, caller);
            return BuildResponse<GetTokenSuppliesReturn>(responseBody, caller);
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
        public static async Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(string chainID, GetTokenSuppliesMapArgs args, int retries = 0, IHttpHandler httpHandler = null, IIndexer caller = null)
        {
            var responseBody = await HttpPost(chainID, "GetTokenSuppliesMap", args, retries, httpHandler, caller);
            return BuildResponse<GetTokenSuppliesMapReturn>(responseBody, caller);
        }

        [Obsolete]
        public static async Task<GetBalanceUpdatesReturn> GetBalanceUpdates(BigInteger chainID, GetBalanceUpdatesArgs args)
        {
            return await GetBalanceUpdates(chainID.ToString(), args);
        }

        [Obsolete]
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
        public static async Task<GetTransactionHistoryReturn> GetTransactionHistory(string chainID, GetTransactionHistoryArgs args, int retries = 0, IHttpHandler httpHandler = null, IIndexer caller = null)
        {
            var responseBody = await HttpPost(chainID, "GetTransactionHistory", args, retries, httpHandler, caller);
            return BuildResponse<GetTransactionHistoryReturn>(responseBody);
        }
        
        /// <summary>
        /// Subscribe to receipt events.
        /// </summary>
        public static void SubscribeReceipts(string chainID, SubscribeReceiptsArgs args, WebRPCStreamOptions<SubscribeReceiptsReturn> options, IIndexer caller)
        {
            new HttpHandler(_builderApiKey, caller).HttpStream(chainID, "SubscribeReceipts", args, options);
        }
        
        /// <summary>
        /// Subscribe to smart contract events.
        /// </summary>
        public static void SubscribeEvents(string chainID, SubscribeEventsArgs args, WebRPCStreamOptions<SubscribeEventsReturn> options, IIndexer caller)
        {
            new HttpHandler(_builderApiKey, caller).HttpStream(chainID, "SubscribeEvents", args, options);
        }
        
        /// <summary>
        /// Subscribe to balance update events for a given contract address.
        /// </summary>
        public static void SubscribeBalanceUpdates(string chainID, SubscribeBalanceUpdatesArgs args, WebRPCStreamOptions<SubscribeBalanceUpdatesReturn> options, IIndexer caller)
        {
            new HttpHandler(_builderApiKey, caller).HttpStream(chainID, "SubscribeBalanceUpdates", args, options);
        }
        
        /// <summary>
        /// Subscribe to balance update events for a given contract address.
        /// </summary>
        public static void AbortStreams(IIndexer caller)
        {
            new HttpHandler(_builderApiKey, caller).AbortStreams();
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
        private static async Task<string> HttpPost(string chainID, string endPoint, object args, int retries = 0, IHttpHandler httpHandler = null, IIndexer caller = null)
        {
            if (httpHandler == null)
            {
                if (string.IsNullOrWhiteSpace(_builderApiKey))
                {
                    throw SequenceConfig.MissingConfigError("Builder API Key");
                }
                
                httpHandler = new HttpHandler(_builderApiKey, caller);
            }

            string result = "";

            try
            {
                result = await httpHandler.HttpPost(chainID, endPoint, args, retries);
            }
            catch (Exception e)
            {
               OnQueryFailed?.Invoke(e.Message);
               if (caller != null)
               {
                   caller.OnQueryFailed(e.Message);
               }
            }

            return result;
        }

        /// <summary>
        /// Parses <paramref name="text"/> into JSON, if it fails due to <paramref name="text"/> not being of JSON format, will throw an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        private static T BuildResponse<T>(string text, IIndexer caller = null)
        {
            try
            {
                T data = JsonConvert.DeserializeObject<T>(text);
                return data;
            }
            catch (Exception e)
            {
                string error =
                    $"Error deserializing response into type: {typeof(T)}, given: {text}, reason: {e.Message}";
                OnQueryFailed?.Invoke(error);
                if (caller != null)
                {
                    caller.OnQueryFailed(error);
                }
            }
            return default;
        }
    }
}