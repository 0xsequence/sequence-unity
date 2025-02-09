using System;
using System.Collections;
using System.Threading.Tasks;
using Sequence;
using Sequence.Demo;
using Sequence.Demo.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SequenceSDK.Samples
{
    public class SequenceDailyRewardTile : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dayText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private TMP_Text _claimHeaderText;
        [SerializeField] private TMP_Text _claimButtonText;
        [SerializeField] private RawImage _tokenImage;
        [SerializeField] private Button _claimButton;
        [SerializeField] private GameObject _hightlight;
        [SerializeField] private GameObject _claimHeader;
        [SerializeField] private GameObject _claimHeaderBg;
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private GameObject _checkmark;
        [SerializeField] private GameObject _doneOverlay;
        [SerializeField] private RectTransform _rectTransform;

        private int _nextClaimTime;
        private int _nextResetTime;
        private TokenMetadata _metadata;
        private Func<Task> _claim;
        private UnityAction _reset;
        private Coroutine _claimTimeRoutine;
        
        public void Show(int i, DailyRewardsStatusData config, RewardData reward, TokenMetadata metadata, Func<Task> claim, UnityAction reset)
        {
            _metadata = metadata;
            _claim = claim;
            _reset = reset;
            _nextClaimTime = config.userStatus.lastClaimTime + config.timeSpan;
            _nextResetTime = config.userStatus.lastClaimTime + config.timeSpan * 2;

            var progress = config.userStatus.progress;
            var curTime = TimeUtils.GetTimestampSecondsNow();
            var canClaim = i == progress && curTime >= _nextClaimTime;
            
            EnableClaimHeader(canClaim);

            _dayText.text = $"Day {i + 1}";
            _claimButton.interactable = canClaim;
            _loadingScreen.SetActive(false);

            var daysLeft = i - progress;
            if (daysLeft == 0 && curTime < _nextResetTime)
                StartClaimTimeRoutine();
            else if (daysLeft < 0)
                _claimButtonText.text = "Claimed";
            else
                _claimButtonText.text = "Claim";

            _checkmark.SetActive(daysLeft < 0);
            _doneOverlay.SetActive(daysLeft < 0);
            EnableClaimButton(daysLeft == 0);
            
            LoadMetadata(reward.amount);
        }

        private async void LoadMetadata(int amount)
        {
            _amountText.text = amount.ToString();
            _nameText.text = _metadata.name;
            _tokenImage.texture = await AssetHandler.GetTexture2DAsync(_metadata.image);
        }

        public async void Claim()
        {
            EnableClaimHeader(false);
            _loadingScreen.SetActive(true);
            _claimButton.interactable = false;
            _claimButtonText.text = "Claiming...";
            
            this.ForceStopCoroutine(ref _claimTimeRoutine);
            await _claim.Invoke();
        }

        private void EnableClaimButton(bool enable)
        {
            var sizeDelta = _rectTransform.sizeDelta;
            sizeDelta.y = enable ? 170 : 140;
            _rectTransform.sizeDelta = sizeDelta;
            _claimButton.gameObject.SetActive(enable);
        }

        private void EnableClaimHeader(bool enable)
        {
            _hightlight.SetActive(enable);
            _claimHeader.SetActive(enable);
            _claimHeaderBg.SetActive(enable);
        }

        private void StartClaimTimeRoutine()
        {
            this.ForceStopCoroutine(ref _claimTimeRoutine);
            _claimTimeRoutine = StartCoroutine(ClaimTimeRoutine());
        }

        private IEnumerator ClaimTimeRoutine()
        {
            var remainingTime = _nextClaimTime - TimeUtils.GetTimestampSecondsNow();
            while (remainingTime > 1)
            {
                remainingTime = _nextClaimTime - TimeUtils.GetTimestampSecondsNow();
                _claimButtonText.text = $"Claim in {TimeUtils.FormatRemainingTime(remainingTime)}";
                yield return new WaitForSeconds(1);
            }

            _claimButton.interactable = true;
            _claimButtonText.text = "Claim";
            EnableClaimHeader(true);
            
            remainingTime = _nextResetTime - TimeUtils.GetTimestampSecondsNow();
            while (remainingTime > 1)
            {
                remainingTime = _nextResetTime - TimeUtils.GetTimestampSecondsNow();
                _claimHeaderText.text = $"{TimeUtils.FormatRemainingTime(remainingTime)} left to Claim";
                yield return new WaitForSeconds(1);
            }

            _reset?.Invoke();
        }
    }
}
