using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace SequenceSDK.Samples
{
    public class SequenceDailyRewards : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private string _apiUrl;
        [SerializeField] private Chain _chain = Chain.TestnetArbitrumSepolia;
        
        [Header("Components")]
        [SerializeField] private GameObject _contentParent;
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private GenericObjectPool<SequenceDailyRewardTile> _tilePool;
        
        private IWallet _wallet;
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void Show(IWallet wallet)
        {
            _wallet = wallet;
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

        private void LoadRewards(DailyRewardsStatusData rewardsData, Dictionary<string, TokenSupply[]> supplies)
        {
            _tilePool.Cleanup();
            for (var index = 0; index < rewardsData.rewards.Count; index++)
            {
                var rewards = rewardsData.rewards[index];
                var reward = rewards[0];
                
                var metadata = supplies[reward.contractAddress].First(s => s.tokenID == reward.tokenId).tokenMetadata;
                _tilePool.GetObject().Show(index, rewardsData.userStatus.progress, rewardsData.nextClaimTime, reward, metadata, ClaimReward);
            }
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
                Debug.Log($"{e.Message} ({e.StackTrace})");
                return false;
            }

            if (!request.isDone || request.result != UnityWebRequest.Result.Success)
                return false;

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
            
            EnableContent(true);
            LoadRewards(rewardsData, supplies);
            return true;
        }
    }
}
