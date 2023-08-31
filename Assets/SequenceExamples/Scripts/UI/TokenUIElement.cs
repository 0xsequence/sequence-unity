using System;
using Sequence.Contracts;
using Sequence.Demo.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence.Demo.Utils;
using UnityEngine.Serialization;

namespace Sequence.Demo
{
    public class TokenUIElement : MonoBehaviour
    {
        [SerializeField] private Image _tokenIconImage;
        [SerializeField] private TextExtender _tokenNameSetter;
        [SerializeField] private Image _networkIcon;
        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private TextMeshProUGUI _currencyValueText;
        [SerializeField] private TextMeshProUGUI _percentChangeText;
        public NetworkIcons NetworkIcons;

        private TokenElement _tokenElement;
        private BalanceCurrencyTextSetter _balanceCurrencyTextSetter;
        private Color _baseColor;
        private SequenceUI _sequenceUI;

        private void Awake()
        {
            _baseColor = _percentChangeText.color;
        }

        public void Assemble(TokenElement tokenElement)
        {
            _tokenElement = tokenElement;
            
            _tokenIconImage.sprite = _tokenElement.TokenIconSprite;
            _tokenNameSetter.SetText(_tokenElement.TokenName);
            _networkIcon.sprite = NetworkIcons.GetIcon(_tokenElement.Network);
            
            _balanceCurrencyTextSetter = new BalanceCurrencyTextSetter(_balanceText, _currencyValueText, _tokenElement, _percentChangeText, _baseColor);
            _balanceCurrencyTextSetter.SetInitialValueAndBalanceText();
        }

        public void RefreshCurrencyValue()
        {
            ThrowIfNotAssembled();
            _balanceCurrencyTextSetter.RefreshCurrencyValue();
        }
        
        public void RefreshWithBalance(uint balance)
        {
            ThrowIfNotAssembled();
            _balanceCurrencyTextSetter.RefreshWithBalance(balance);
        }

        private void ThrowIfNotAssembled()
        {
            if (_balanceCurrencyTextSetter == null)
            {
                throw new SystemException(
                    $"{typeof(TokenUIElement)} must be assembled via {nameof(Assemble)} before use.");
            }
        }

        public Chain GetNetwork()
        {
            return _tokenElement.Network;
        }

        public void SwitchToInfoPage()
        {
            if (_sequenceUI == null)
            {
                _sequenceUI = FindObjectOfType<SequenceUI>();
            }

            _sequenceUI.SwitchToTokenInfoPage(_tokenElement);
        }
    }
}
