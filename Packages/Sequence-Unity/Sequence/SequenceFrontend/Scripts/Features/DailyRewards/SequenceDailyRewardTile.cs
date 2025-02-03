using System;
using System.Collections;
using System.Threading.Tasks;
using Sequence;
using Sequence.Demo;
using Sequence.Demo.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceSDK.Samples
{
    public class SequenceDailyRewardTile : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dayText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _claimButtonText;
        [SerializeField] private RawImage _tokenImage;
        [SerializeField] private Button _claimButton;
        [SerializeField] private GameObject _hightlight;
        [SerializeField] private GameObject _claimHeader;
        [SerializeField] private GameObject _loadingScreen;

        private int _nextClaimTime;
        private TokenMetadata _metadata;
        private Func<Task> _claim;
        private Coroutine _claimTimeRoutine;
        
        public void Show(int i, int progress, int nextClaimTime, RewardData reward, TokenMetadata metadata, Func<Task> claim)
        {
            _nextClaimTime = nextClaimTime;
            _metadata = metadata;
            var curTime = TimeUtils.GetTimestampSecondsNow();
            var canClaim = i == progress && curTime >= nextClaimTime;
            _dayText.gameObject.SetActive(!canClaim);
            _hightlight.SetActive(canClaim);
            _claimHeader.SetActive(canClaim);
            
            _claim = claim;
            _dayText.text = $"Day {i + 1}";
            transform.localScale = Vector3.one * (i == progress ? 1.1f : 1);
            _claimButton.interactable = canClaim;
            _loadingScreen.SetActive(false);
            
            if (i == progress && curTime < nextClaimTime)
                StartClaimTimeRoutine();

            LoadMetadata(reward.amount);
        }

        private async void LoadMetadata(int amount)
        {
            _nameText.text = $"{amount}x {_metadata.name}";
            _tokenImage.texture = await AssetHandler.GetTexture2DAsync(_metadata.image);
        }

        public async void Claim()
        {
            _loadingScreen.SetActive(true);
            _claimButton.interactable = false;
            await _claim.Invoke();
        }

        private void StartClaimTimeRoutine()
        {
            if (_claimTimeRoutine != null)
                StopCoroutine(_claimTimeRoutine);

            _claimTimeRoutine = StartCoroutine(ClaimTimeRoutine());
        }

        private IEnumerator ClaimTimeRoutine()
        {
            var remainingTime = _nextClaimTime - TimeUtils.GetTimestampSecondsNow();
            while (remainingTime > 0)
            {
                remainingTime = _nextClaimTime - TimeUtils.GetTimestampSecondsNow();
                _claimButtonText.text = $"Claim in {TimeUtils.FormatRemainingTime(remainingTime)}";
                yield return new WaitForSeconds(1);
            }

            _claimButton.interactable = true;
            _claimButtonText.text = "Claim";
            _dayText.gameObject.SetActive(false);
            _hightlight.SetActive(true);
            _claimHeader.SetActive(true);
        }
    }
}
