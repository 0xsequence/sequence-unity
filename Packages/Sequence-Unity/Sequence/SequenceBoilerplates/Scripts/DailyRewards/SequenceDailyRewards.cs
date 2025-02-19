using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EmbeddedWallet;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Boilerplates.DailyRewards
{
    public class SequenceDailyRewards : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject _contentParent;
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private GenericObjectPool<SequenceDailyRewardTile> _tilePool;
        
        private IWallet _wallet;
        private Chain _chain;
        private string _apiUrl;
        private Action _onClose;
        private DailyRewardsStatusData _rewardsData;
        private Dictionary<string, TokenSupply[]> _supplies;
        
        /// <summary>
        /// This function is called when the user clicks the close button.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            _onClose?.Invoke();
        }
        
        /// <summary>
        /// Required function to configure this Boilerplate.
        /// </summary>
        /// <param name="wallet">This Wallet instance will perform transactions.</param>
        /// <param name="chain">Chain used to get balances and send transactions.</param>
        /// <param name="apiUrl">API Url you deployed using the server boilerplate.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        public void Show(IWallet wallet, Chain chain, string apiUrl, Action onClose = null)
        {
            _wallet = wallet;
            _chain = chain;
            _apiUrl = apiUrl;
            _onClose = onClose;
            gameObject.SetActive(true);
            _messagePopup.gameObject.SetActive(false);
            GetRewards();
        }

        private void EnableContent(bool enable)
        {
            _contentParent.SetActive(enable);
            _loadingScreen.SetActive(!enable);
        }

        private async void GetRewards()
        {
            EnableContent(false);
            await CallRewardsAsync(UnityWebRequest.kHttpVerbGET);
        }

        private async Task ClaimReward()
        {
            var success = await CallRewardsAsync(UnityWebRequest.kHttpVerbPOST);
            if (success)
                _messagePopup.Show("Claimed.");
            else
                _messagePopup.Show("Failed.", true);
        }

        private void LoadRewards()
        {
            _tilePool.Cleanup();
            for (var index = 0; index < _rewardsData.rewards.Count; index++)
            {
                var rewards = _rewardsData.rewards[index];
                var reward = rewards[0];
                
                var metadata = _supplies[reward.contractAddress]
                    .First(s => s.tokenID == reward.tokenId).tokenMetadata;
                
                _tilePool.GetObject()
                    .Show(index, _rewardsData, reward, metadata, ClaimReward, ResetRewards);
            }
        }

        private void ResetRewards()
        {
            _rewardsData.userStatus.progress = 0;
            LoadRewards();
        }

        private async Task<bool> CallRewardsAsync(string method)
        {
            var request = UnityWebRequest.Get(_apiUrl);
            request.method = method;
            
            var idToken = await _wallet.GetIdToken();
            request.SetRequestHeader("Authorization", $"Bearer {idToken.IdToken}");
            
            try
            {
                await request.SendWebRequest();
            }
            catch (Exception e)
            {
                Debug.Log($"{e.Message} {request.downloadHandler.text}");
                _messagePopup.Show(e.Message, true);
                return false;
            }

            if (!request.isDone || request.result != UnityWebRequest.Result.Success)
            {
                _messagePopup.Show("Request failed.", true);
                return false;
            }

            var json = request.downloadHandler.text;
            var rewardsData = JsonConvert.DeserializeObject<DailyRewardsStatusData>(json);

            var dict = rewardsData.rewards.GroupBy(reward => reward[0].contractAddress)
                .ToDictionary(group => group.Key, 
                    group => group.Select(e => e[0].tokenId.ToString())
                        .Distinct()
                        .ToArray());

            var indexer = new ChainIndexer(_chain);
            var args = new GetTokenSuppliesMapArgs
            {
                tokenMap = dict,
                includeMetadata = true
            };
            
            var metadata = await indexer.GetTokenSuppliesMap(args);
            var supplies = metadata.supplies;

            _rewardsData = rewardsData;
            _supplies = supplies;
            
            EnableContent(true);
            LoadRewards();
            return true;
        }
    }
}
