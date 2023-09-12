using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.Demo
{
    public class InfoPage : PageWithTransactionDetailsBlocks
    {
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private TextMeshProUGUI _currencyValueText;
        public float TimeBetweenCurrencyValueRefreshesInSeconds = 5;

        private AmountAndCurrencyTextSetter _amountAndCurrencyTextSetter;

        public override void Open(params object[] args)
        {
            base.Open(args);
            
            Assemble(); 
            StartCoroutine(RefreshCurrencyValueRepeatedly());
        }

        private void Assemble()
        {
            _amountAndCurrencyTextSetter = new AmountAndCurrencyTextSetter(_amountText, _currencyValueText, _currencyRepository);
            _amountAndCurrencyTextSetter.SetInitialValueAndAmountText();
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
                    $"{GetType().Name} must be assembled via {nameof(Assemble)} before use.");
            }
        }

        private IEnumerator RefreshCurrencyValueRepeatedly()
        {
            var waitForRefresh = new WaitForSecondsRealtime(TimeBetweenCurrencyValueRefreshesInSeconds);
            while (true) // Terminates on Close() (as this gameObject will be disabled)
            {
                yield return waitForRefresh;
                RefreshCurrencyValue();
            }
        }

        public Sprite GetNetworkIcon(Chain network)
        {
            return _networkIcons.GetIcon(network);
        }
    }
}