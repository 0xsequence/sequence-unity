using System;
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
        
        private RectTransform _arrowIconRectTransform;
        private TransactionDetails _transactionDetails;
        private NetworkIcons _networkIconsMapper;

        private void Awake()
        {
            _arrowIconRectTransform = _arrowIcon.GetComponent<RectTransform>();
        }

        public void Assemble(TransactionDetails transactionDetails, NetworkIcons networkIcons)
        {
            _transactionDetails = transactionDetails;
            _networkIconsMapper = networkIcons;
            
            _sentReceivedText.SetText(_transactionDetails.Type, resizeWidth: true);
            _networkIcon.sprite = _networkIconsMapper.GetIcon(_transactionDetails.Network);
            _tokenIcon.sprite = _transactionDetails.TokenIcon;
            _amountText.text =
                $"{_transactionDetails.Amount.AppendSignIfNeeded()}{_transactionDetails.Amount:N2} {_transactionDetails.Symbol}";
            _dateText.text = _transactionDetails.Date;
            CurrencyValue currencyValue = _transactionDetails.CurrencyConverter.ConvertToCurrency(_transactionDetails.Amount, _transactionDetails.ContractAddress);
            _currencyValueText.text = $"{currencyValue.Symbol}{currencyValue.Amount:N2}";
        }

        private void ThrowIfNotAssembled()
        {
            if (_transactionDetails == null)
            {
                throw new SystemException(
                    $"{typeof(TransactionDetailsBlock)} must be assembled via {nameof(Assemble)} before use.");
            }
        }
    }
}