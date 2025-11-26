using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Sequence.Sidekick.Config;

namespace Sequence.Sidekick
{
    public class SequenceSidekickClient
    {
        private static readonly System.Net.Http.HttpClient client = new();
        private string baseUrl = "http://localhost:3000";

        public Chain Chain;

        private string ChainId => Chain.GetChainId();

        public SequenceSidekickClient() { }

        public SequenceSidekickClient(Chain chain)
        {
            Chain = chain;
        }

        #region HttpRequests
        private async Task<string> GetAsync(string endpoint, Dictionary<string, string> headers = null)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseUrl + endpoint))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        private async Task<string> PostAsync(string endpoint, string json)
        {
            string url = baseUrl + endpoint;

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Add("x-secret-key", Resources.Load<SidekickConfig>("SidekickConfig").SecretKey);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = content;

                try
                {
                    HttpResponseMessage response = await client.SendAsync(request);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    return responseContent;
                }
                catch (HttpRequestException e)
                {
                    throw new System.Exception(e.Message);
                }
            }
        }

        #endregion

        #region Wallet
        public async Task<string> GetWalletAddress()
        {
            return await GetAsync("/sidekick/wallet-address");
        }
        #endregion

        #region Contract
        public async Task<string> ReadContract(string address, string functionName, string json)
        {
            return await PostAsync($"/read/contract/{ChainId}/{address}/{functionName}", json);
        }

        public async Task<string> WriteContract(string address, string functionName, string json)
        {
            return await PostAsync($"/write/contract/{ChainId}/{address}/{functionName}", json);
        }

        public async Task<string> GetAllContracts()
        {
            return await GetAsync("/contract/getAll");
        }

        public async Task<string> GetContract(string address)
        {
            return await GetAsync($"/contract/get/{address}");
        }

        public async Task<string> IsContractDeployed(string address)
        {
            return await GetAsync($"/contract/isDeployed/{ChainId}/{address}");
        }

        public async Task<string> DeployContract(string json)
        {
            return await PostAsync($"/deploy/contract/{ChainId}", json);
        }

        public async Task<string> ImportContractsFromBuilder(string projectId, string json)
        {
            return await PostAsync($"/contract/importAllFromBuilder/{projectId}", json);
        }

        public async Task<string> AddContract(string json)
        {
            return await PostAsync("/contract/add", json);
        }
        #endregion

        #region ERC20
        public async Task<string> TransferERC20(string address, string json)
        {
            return await PostAsync($"/write/erc20/{ChainId}/{address}/transfer", json);
        }

        public async Task<string> ApproveERC20(string address, string json)
        {
            return await PostAsync($"/write/erc20/{ChainId}/{address}/approve", json);
        }

        public async Task<string> MintERC20(string address, string json)
        {
            return await PostAsync($"/write/erc20/{ChainId}/{address}/mint", json);
        }

        public async Task<string> TransferFromERC20(string address, string json)
        {
            return await PostAsync($"/write/erc20/{ChainId}/{address}/transferFrom", json);
        }

        public async Task<string> DeployERC20(string json)
        {
            return await PostAsync($"/deploy/erc20/{ChainId}", json);
        }
        #endregion

        #region ERC721
        public async Task<string> SafeMintERC721(string address, string json)
        {
            return await PostAsync($"/write/erc721/{ChainId}/{address}/safeMint", json);
        }

        public async Task<string> SafeMintBatchERC721(string address, string json)
        {
            return await PostAsync($"/write/erc721/{ChainId}/{address}/safeMintBatch", json);
        }

        public async Task<string> BalanceOfERC721(string address)
        {
            return await GetAsync($"/read/erc721/{ChainId}/{address}/balanceOf");
        }

        public async Task<string> DeployERC721(string json)
        {
            return await PostAsync($"/deploy/erc721/{ChainId}", json);
        }
        #endregion

        #region ERC1155
        public async Task<string> MintERC1155(string address, string json)
        {
            return await PostAsync($"/write/erc1155/{ChainId}/{address}/mint", json);
        }

        public async Task<string> MintBatchERC1155(string address, string json)
        {
            return await PostAsync($"/write/erc1155/{ChainId}/{address}/mintBatch", json);
        }

        public async Task<string> GrantRoleERC1155(string address, string json)
        {
            return await PostAsync($"/write/erc1155/{ChainId}/{address}/grantRole", json);
        }

        public async Task<string> HasRoleERC1155(string address)
        {
            return await GetAsync($"/read/erc1155/{ChainId}/{address}/hasRole");
        }

        public async Task<string> MinterRoleERC1155(string address)
        {
            return await GetAsync($"/read/erc1155/{ChainId}/{address}/minterRole");
        }

        public async Task<string> BalanceOfERC1155(string address)
        {
            return await GetAsync($"/read/erc1155/{ChainId}/{address}/balanceOf");
        }

        public async Task<string> DeployERC1155(string json)
        {
            return await PostAsync($"/deploy/erc1155/{ChainId}", json);
        }
        #endregion

        #region Transactions
        public async Task<string> GetAllTransactions()
        {
            return await GetAsync("/transactions");
        }

        public async Task<string> GetTransaction(string txHash)
        {
            return await GetAsync($"/transactions/{txHash}");
        }
        #endregion

        #region Webhooks
        public async Task<string> AddWebhook(string json)
        {
            return await PostAsync("/webhook/add", json);
        }

        public async Task<string> RemoveWebhook(string json)
        {
            return await PostAsync("/webhook/remove", json);
        }

        public async Task<string> RemoveAllWebhooks()
        {
            return await PostAsync("/webhook/removeAll", "");
        }

        public async Task<string> GetAllWebhooks()
        {
            return await GetAsync("/webhook/getAll");
        }
        #endregion

        #region Jobs
        public async Task<string> StartERC20RewardsJob(string address, string json)
        {
            return await PostAsync($"/jobs/erc20/rewards/{ChainId}/{address}/start", json);
        }

        public async Task<string> StopERC20RewardsJob(string address, string json)
        {
            return await PostAsync($"/jobs/erc20/rewards/{ChainId}/{address}/stop", json);
        }

        public async Task<string> GetAllJobs()
        {
            return await GetAsync("/jobs");
        }

        public async Task<string> GetJob(string jobId)
        {
            return await GetAsync($"/jobs/{jobId}");
        }

        public async Task<string> CleanJobs(string json)
        {
            return await PostAsync("/jobs/clean", json);
        }
        #endregion
    }
}
