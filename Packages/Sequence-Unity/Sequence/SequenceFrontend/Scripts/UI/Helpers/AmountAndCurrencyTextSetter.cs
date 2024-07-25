using JetBrains.Annotations;
using Sequence.Demo.Utils;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class AmountAndCurrencyTextSetter
    {
        private ITextSetter _amountText;
        private TextMeshProUGUI _currencyValueText;
        private ICurrencyRepository _currencyRepository;
        private TextMeshProUGUI _percentChangeText;
        private Color _baseColor;

        public AmountAndCurrencyTextSetter(ITextSetter amountText, TextMeshProUGUI currencyValueText, ICurrencyRepository currencyRepository, [CanBeNull] TextMeshProUGUI percentChangeText = null, Color? baseColor = null)
        {
            _amountText = amountText;
            _currencyValueText = currencyValueText;
            _currencyRepository = currencyRepository;
            _percentChangeText = percentChangeText;
            if (baseColor != null)
            {
                _baseColor = (Color)baseColor;
            }
        }
        
        public AmountAndCurrencyTextSetter(TextMeshProUGUI amountText, TextMeshProUGUI currencyValueText, ICurrencyRepository currencyRepository, [CanBeNull] TextMeshProUGUI percentChangeText = null, Color? baseColor = null)
        {
            _amountText = new TextSetter(amountText);
            _currencyValueText = currencyValueText;
            _currencyRepository = currencyRepository;
            _percentChangeText = percentChangeText;
            if (baseColor != null)
            {
                _baseColor = (Color)baseColor;
            }
        }

        public void SetInitialValueAndAmountText()
        {
            _amountText.SetText($"{_currencyRepository.GetAmount():#,##0} {_currencyRepository.GetSymbol()}", true);
            
            Currency currency = _currencyRepository.GetCurrency();
            float amount = currency.Amount;
            
            _currencyValueText.text = $"{currency.Symbol}{amount:N2}";

            if (_percentChangeText != null)
            {
                _currencyRepository.SetPreviousCurrencyValue(amount);
                _percentChangeText.text = "0.00%";
                _percentChangeText.color = _baseColor;
            }
        }

        public void RefreshCurrencyValue()
        {
            Currency currency = _currencyRepository.GetCurrency();
            float amount = currency.Amount;
            
            _currencyValueText.text = $"{currency.Symbol}{amount:N2}";

            if (_percentChangeText != null)
            {
                float change = amount - _currencyRepository.GetPreviousCurrencyValue();
                _percentChangeText.text =
                    $"{change.AppendSignIfNeeded()}{(change) / _currencyRepository.GetPreviousCurrencyValue() * 100:N2}%";
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

            _currencyRepository.SetPreviousCurrencyValue(amount);
        }

        public void RefreshWithAmount(uint amount)
        {
            if (amount == _currencyRepository.GetAmount())
            {
                return;
            }
            _currencyRepository.SetAmount(amount);
            
            SetInitialValueAndAmountText();
        }
    }
}