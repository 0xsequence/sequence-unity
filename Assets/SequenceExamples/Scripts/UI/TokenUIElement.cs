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

        public void Assemble(TokenElement tokenElement)
        {
            _tokenElement = tokenElement;
            
            _tokenIcon.sprite = _tokenElement.TokenIconSprite;
            _tokenNameSetter.SetText(_tokenElement.TokenName);
            _networkIcon.sprite = _tokenElement.NetworkIconSprite;
            
            _isAssembled = true;
            
            RefreshWithBalance(_tokenElement.Balance);
        }

        public void RefreshWithBalance(uint balance)
        {
            ThrowIfNotAssembled();
            _tokenElement.Balance = balance;
            
            CurrencyValue currencyValue = _tokenElement.CurrencyConverter.ConvertToCurrency(_tokenElement.Balance, _tokenElement.Erc20);
            float amount = currencyValue.Amount;
            
            _balanceText.text = $"{_tokenElement.Balance} {_tokenElement.Symbol}";
            _currencyValueText.text = $"{currencyValue.Symbol}{amount:N2}";
            _percentChangeText.text =
                $"{amount.GetSignAsString()}{(amount - _tokenElement.PreviousCurrencyValue) / _tokenElement.PreviousCurrencyValue * 100}%";

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
