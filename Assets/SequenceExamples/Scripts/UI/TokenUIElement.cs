using System;
using Sequence.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence.Demo.Utils;

namespace Sequence.Demo
{
    public class TokenUIElement : MonoBehaviour
    {
        [SerializeField] private Image _tokenIcon;
        [SerializeField] private TextExtender _tokenNameSetter;
        [SerializeField] private Image _networkIcon;
        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private TextMeshProUGUI _currencyValueText;
        [SerializeField] private TextMeshProUGUI _percentChangeText;

        private TokenElement _tokenElement;

        private bool _isAssembled = false;

        private Color _baseColor;

        private void Awake()
        {
            _baseColor = _percentChangeText.color;
        }

        public void Assemble(TokenElement tokenElement)
        {
            _tokenElement = tokenElement;
            
            _tokenIcon.sprite = _tokenElement.TokenIconSprite;
            _tokenNameSetter.SetText(_tokenElement.TokenName);
            _networkIcon.sprite = _tokenElement.NetworkIconSprite;
            
            _isAssembled = true;
            
            SetInitialValueAndBalanceText();
        }

        public void RefreshWithBalance(uint balance)
        {
            ThrowIfNotAssembled();
            if (balance == _tokenElement.Balance)
            {
                return;
            }
            _tokenElement.Balance = balance;
            
            SetInitialValueAndBalanceText();
        }

        private void SetInitialValueAndBalanceText()
        {
            _balanceText.text = $"{_tokenElement.Balance} {_tokenElement.Symbol}";
            
            CurrencyValue currencyValue = _tokenElement.CurrencyConverter.ConvertToCurrency(_tokenElement.Balance, _tokenElement.Erc20);
            float amount = currencyValue.Amount;
            _tokenElement.PreviousCurrencyValue = amount;
            
            _currencyValueText.text = $"{currencyValue.Symbol}{amount:N2}";
            _percentChangeText.text = "0.00%";
            _percentChangeText.color = _baseColor;
        }

        public void RefreshCurrencyValue()
        {
            ThrowIfNotAssembled();
            CurrencyValue currencyValue = _tokenElement.CurrencyConverter.ConvertToCurrency(_tokenElement.Balance, _tokenElement.Erc20);
            float amount = currencyValue.Amount;
            
            _currencyValueText.text = $"{currencyValue.Symbol}{amount:N2}";
            float change = amount - _tokenElement.PreviousCurrencyValue;
            _percentChangeText.text =
                $"{amount.GetSignAsString()}{(change) / _tokenElement.PreviousCurrencyValue * 100:N2}%";
            if (change > 0)
            {
                _percentChangeText.color = Color.green;
            } else if (change == 0)
            {
                _percentChangeText.color = _baseColor;
            }
            else
            {
                _percentChangeText.color = Color.red;
            }

            _tokenElement.PreviousCurrencyValue = amount;
        }

        private void ThrowIfNotAssembled()
        {
            if (!_isAssembled)
            {
                throw new SystemException(
                    $"{typeof(TokenUIElement)} must be assembled via {nameof(Assemble)} before use.");
            }
        }
    }

    public class TokenElement
    {
        public ERC20 Erc20;
        public Sprite TokenIconSprite;
        public string TokenName;
        public Sprite NetworkIconSprite;
        public uint Balance;
        public string Symbol;
        public ICurrencyConverter CurrencyConverter;
        public float PreviousCurrencyValue;
        
        public TokenElement(ERC20 erc20, Sprite tokenIconSprite, string tokenName, Sprite networkIconSprite, uint balance, string symbol, ICurrencyConverter currencyConverter)
        {
            Erc20 = erc20;
            TokenIconSprite = tokenIconSprite;
            TokenName = tokenName;
            NetworkIconSprite = networkIconSprite;
            Balance = balance;
            Symbol = symbol;
            CurrencyConverter = currencyConverter;
            PreviousCurrencyValue = currencyConverter.ConvertToCurrency(balance, erc20).Amount;
        }

        public TokenElement(string tokenAddress, Sprite tokenIconSprite, string tokenName, Sprite networkIconSprite,
            uint balance, string symbol, ICurrencyConverter currencyConverter)
        {
            Erc20 = new ERC20(tokenAddress);
            TokenIconSprite = tokenIconSprite;
            TokenName = tokenName;
            NetworkIconSprite = networkIconSprite;
            Balance = balance;
            Symbol = symbol;
            CurrencyConverter = currencyConverter;
            PreviousCurrencyValue = currencyConverter.ConvertToCurrency(balance, Erc20).Amount;
        }
    }
}
