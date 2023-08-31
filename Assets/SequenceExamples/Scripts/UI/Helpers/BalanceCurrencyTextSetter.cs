using JetBrains.Annotations;
using Sequence.Demo.Utils;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class BalanceCurrencyTextSetter
    {
        private TextMeshProUGUI _balanceText;
        private TextMeshProUGUI _currencyValueText;
        private TokenElement _tokenElement;
        private TextMeshProUGUI _percentChangeText;
        private Color _baseColor;

        public BalanceCurrencyTextSetter(TextMeshProUGUI balanceText, TextMeshProUGUI currencyValueText, TokenElement tokenElement, [CanBeNull] TextMeshProUGUI percentChangeText = null, Color? baseColor = null)
        {
            _balanceText = balanceText;
            _currencyValueText = currencyValueText;
            _tokenElement = tokenElement;
            _percentChangeText = percentChangeText;
            if (baseColor != null)
            {
                _baseColor = (Color)baseColor;
            }
        }

        public void SetInitialValueAndBalanceText()
        {
            _balanceText.text = $"{_tokenElement.Balance:#,##0} {_tokenElement.Symbol}";
            
            CurrencyValue currencyValue = _tokenElement.CurrencyConverter.ConvertToCurrency(_tokenElement.Balance, _tokenElement.Erc20);
            float amount = currencyValue.Amount;
            
            _currencyValueText.text = $"{currencyValue.Symbol}{amount:N2}";

            if (_percentChangeText != null)
            {
                _tokenElement.PreviousCurrencyValue = amount;
                _percentChangeText.text = "0.00%";
                _percentChangeText.color = _baseColor;
            }
        }

        public void RefreshCurrencyValue()
        {
            CurrencyValue currencyValue = _tokenElement.CurrencyConverter.ConvertToCurrency(_tokenElement.Balance, _tokenElement.Erc20);
            float amount = currencyValue.Amount;
            
            _currencyValueText.text = $"{currencyValue.Symbol}{amount:N2}";

            if (_percentChangeText != null)
            {
                float change = amount - _tokenElement.PreviousCurrencyValue;
                _percentChangeText.text =
                    $"{change.AppendSignIfNeeded()}{(change) / _tokenElement.PreviousCurrencyValue * 100:N2}%";
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
            }

            _tokenElement.PreviousCurrencyValue = amount;
        }

        public void RefreshWithBalance(uint balance)
        {
            if (balance == _tokenElement.Balance)
            {
                return;
            }
            _tokenElement.Balance = balance;
            
            SetInitialValueAndBalanceText();
        }
    }
}