using System;
using System.Collections;
using Sequence.Demo.ScriptableObjects;
using Sequence.Demo.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class TransactionDetailsBlock : MonoBehaviour
    {
        [SerializeField] private Image _arrowIcon;
        [SerializeField] private TextExtender _sentReceivedText;
        [SerializeField] private Image _networkIcon;
        [SerializeField] private Image _tokenIcon;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private TextMeshProUGUI _dateText;
        [SerializeField] private TextMeshProUGUI _currencyValueText;
        public float TimeBetweenTokenValueRefreshesInSeconds = 5;
        
        private RectTransform _arrowIconRectTransform;
        private TransactionDetails _transactionDetails;
        private NetworkIcons _networkIconsMapper;
        private AmountAndCurrencyTextSetter _amountAndCurrencyTextSetter;

        private void Awake()
        {
            _arrowIconRectTransform = _arrowIcon.GetComponent<RectTransform>();
        }

        public void Assemble(TransactionDetails transactionDetails, NetworkIcons networkIcons)
        {
            StopAllCoroutines();
            _transactionDetails = transactionDetails;
            _networkIconsMapper = networkIcons;
            
            _sentReceivedText.SetText(_transactionDetails.Type, resizeWidth: true);
            _networkIcon.sprite = GetNetworkIcon();
            _tokenIcon.sprite = _transactionDetails.TokenIcon;
            _dateText.text = _transactionDetails.Date;

            _amountAndCurrencyTextSetter = new AmountAndCurrencyTextSetter(_amountText, _currencyValueText, _transactionDetails);
            _amountAndCurrencyTextSetter.SetInitialValueAndAmountText();

            StartCoroutine(ContinuouslyRefreshCurrencyValue());
        }

        private void ThrowIfNotAssembled()
        {
            if (_transactionDetails == null)
            {
                throw new SystemException(
                    $"{typeof(TransactionDetailsBlock)} must be assembled via {nameof(Assemble)} before use.");
            }
        }

        public void RefreshCurrencyValue()
        {
            ThrowIfNotAssembled();
            _amountAndCurrencyTextSetter.RefreshCurrencyValue();
        }

        private IEnumerator ContinuouslyRefreshCurrencyValue()
        {
            var wait = new WaitForSecondsRealtime(TimeBetweenTokenValueRefreshesInSeconds);
            while (true)
            {
                yield return wait;
                RefreshCurrencyValue();
            }
        }

        public Sprite GetNetworkIcon()
        {
            ThrowIfNotAssembled();
            return _networkIconsMapper.GetIcon(_transactionDetails.Network);
        }
    }
}