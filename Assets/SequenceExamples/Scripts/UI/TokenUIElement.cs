using System;
using System.Collections;
using Sequence.Contracts;
using Sequence.Demo.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence.Demo.Utils;
using Sequence.Utils;
using UnityEngine.Serialization;

namespace Sequence.Demo
{
    public class TokenUIElement : WalletUIElement
    {
        [SerializeField] private Image _tokenIconImage;
        [SerializeField] private TextExtender _tokenNameSetter;
        [SerializeField] private Image _networkIcon;
        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private TextMeshProUGUI _currencyValueText;
        [SerializeField] private TextMeshProUGUI _percentChangeText;

        private TokenElement _tokenElement;
        private AmountAndCurrencyTextSetter _amountAndCurrencyTextSetter;
        private Color _baseColor;
        private WalletPanel _walletPanel;

        private void Awake()
        {
            _baseColor = _percentChangeText.color;
        }

        public void Assemble(TokenElement tokenElement, WalletPanel panel)
        {
            _tokenElement = tokenElement;
            
            _tokenIconImage.sprite = _tokenElement.TokenIconSprite;
            _tokenNameSetter.SetText(_tokenElement.TokenName, resizeWidth: true);
            _networkIcon.sprite = NetworkIcons.GetIcon(_tokenElement.Network);
            
            _amountAndCurrencyTextSetter = new AmountAndCurrencyTextSetter(_balanceText, _currencyValueText, _tokenElement, _percentChangeText, _baseColor);
            _amountAndCurrencyTextSetter.SetInitialValueAndAmountText();

            _walletPanel = panel;
        }

        public void RefreshCurrencyValue()
        {
            ThrowIfNotAssembled();
            _amountAndCurrencyTextSetter.RefreshCurrencyValue();
        }
        
        public void RefreshWithBalance(uint balance)
        {
            ThrowIfNotAssembled();
            _amountAndCurrencyTextSetter.RefreshWithAmount(balance);
        }

        private void ThrowIfNotAssembled()
        {
            if (_amountAndCurrencyTextSetter == null)
            {
                throw new SystemException(
                    $"{typeof(TokenUIElement)} must be assembled via {nameof(Assemble)} before use.");
            }
        }

        public override Chain GetNetwork()
        {
            return _tokenElement.Network;
        }

        public void SwitchToInfoPage()
        {
            _walletPanel.OpenTokenInfoPage(_tokenElement, NetworkIcons, TransactionDetailsFetcher);
        }
    }
}
